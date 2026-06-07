using DeltaFour.Application.Dtos;
using DeltaFour.Application.Services;
using DeltaFour.CrossCutting.Middleware;
using DeltaFour.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeltaFour.API.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController(IAuthService service) : Controller
    {
        /// <summary>
        /// Realiza o login do usuário usando suas credenciais e retorna os tokens de autenticação via cookies.
        /// </summary>
        /// <remarks>
        /// Este endpoint valida o usuário, gera o token JWT e o token de refresh,
        /// e os envia no response através de cookies seguros.
        /// </remarks>
        [HttpPost("login")]
        public async Task<ActionResult<UserInfoLoginDto>> Login([FromBody] LoginDto loginDto)
        {
            var user = await service.Login(loginDto);
            if (user == null)
            {
                return BadRequest("Ops, Usuario não existe");
            }

            string jwt = service.CreateToken(user);
            Guid refreshToken = await service.CreateRefreshToken(user.Id, jwt);

            CookieOptions options = Cookie();

            Response.Cookies.Append("Jwt", jwt, options);
            Response.Cookies.Append("RefreshToken", refreshToken.ToString(), options);
            return Ok(service.MapUserInfo(user));
        }

        /// <summary>
        /// Verifica se o usuário ainda possui uma sessão autenticada.
        /// </summary>
        /// <remarks>
        /// Caso o token JWT enviado via cookie ainda esteja válido, retorna 204 (NoContent).
        /// Caso contrário, retorna 403 (Forbid).
        /// </remarks>
        [HttpGet("check-session")]
        public IActionResult CheckSession()
        {
            var user = HttpContext.User;
            if (user?.Identity?.IsAuthenticated == true)
            {
                return NoContent();
            }
            return Forbid();
        }

        /// <summary>
        /// Revalida a sessão do usuário gerando um novo JWT a partir do refresh token.
        /// </summary>
        /// <remarks>
        /// O refresh token é lido dos cookies e, se válido, gera um novo JWT.
        /// Caso o refresh token esteja expirado ou inválido, retorna 403.
        /// </remarks>
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var cookieRefresh = Request.Cookies["RefreshToken"];
            var cookieToken = Request.Cookies["Jwt"];

            if (string.IsNullOrEmpty(cookieRefresh) || string.IsNullOrEmpty(cookieToken))
            {
                return Forbid();
            }

            if (!Guid.TryParse(cookieRefresh, out _))
            {
                return Forbid();
            }

            var jwt = await service.RemakeToken(cookieRefresh, cookieToken);
            if (jwt != null)
            {
                var refreshToken = await service.RemakeRefreshToken(jwt);
                Response.Cookies.Append("RefreshToken", refreshToken.ToString(), Cookie());
                Response.Cookies.Append("Jwt", jwt, Cookie());
                return NoContent();
            }
            return Forbid();
        }

        /// <summary>
        /// Realiza o logout do usuário removendo todos os cookies de autenticação.
        /// </summary>
        /// <remarks>
        /// Deleta o JWT e o refresh token armazenados nos cookies e invalida o refresh token no servidor.
        /// </remarks>
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["RefreshToken"];

            if (!string.IsNullOrEmpty(refreshToken) && Guid.TryParse(refreshToken, out _))
            {
                await service.Logout(refreshToken);
            }

            Response.Cookies.Delete("Jwt");
            Response.Cookies.Delete("RefreshToken");
            return NoContent();
        }

        /// <summary>
        /// Altera a senha do usuário autenticado, exigindo a senha atual.
        /// </summary>
        /// <remarks>
        /// O usuário precisa estar logado. Informe a senha atual e a nova senha.
        /// </remarks>
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var user = HttpContext.GetUserAuthenticated<UserContext>();
            await service.ChangePassword(user.Id, dto);
            return NoContent();
        }

        /// <summary>
        /// Inicia a recuperação de senha enviando um código por e-mail.
        /// </summary>
        /// <remarks>
        /// Caso o e-mail exista, um código de 6 dígitos válido por 30 minutos é enviado.
        /// A resposta é sempre a mesma para não revelar se o e-mail está cadastrado.
        /// </remarks>
        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            await service.ForgotPassword(dto);
            return Ok(new { message = "Se o e-mail estiver cadastrado, um código de recuperação foi enviado." });
        }

        /// <summary>
        /// Redefine a senha a partir do código recebido por e-mail.
        /// </summary>
        /// <remarks>
        /// Informe o e-mail, o código recebido e a nova senha.
        /// </remarks>
        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            await service.ResetPassword(dto);
            return Ok(new { message = "Senha redefinida com sucesso." });
        }

        ///<summary>
        ///Internal generalized method for configuration of cookies
        ///</summary>
        [ApiExplorerSettings(IgnoreApi = true)]
        public CookieOptions Cookie()
        {
            var cookie = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,  // ✅ Ativa para HTTPS
                SameSite = SameSiteMode.None,  // ✅ Permite cross-origin
                IsEssential = true,  // ✅ Garante que será enviado
            };

            return cookie;
        }
    }
}