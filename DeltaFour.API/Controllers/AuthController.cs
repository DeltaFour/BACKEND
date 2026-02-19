using Microsoft.AspNetCore.Mvc;
using DeltaFour.Application.Dtos;
using DeltaFour.Application.Service;
using DeltaFour.CrossCutting.Middleware;
using DeltaFour.Domain.Entities;
using Microsoft.AspNetCore.Authorization;

namespace DeltaFour.API.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController(AuthService service) : Controller
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
            var cookieRefresh = Request.Cookies["RefreshToken"]!;
            String cookieToken = Request.Cookies["Jwt"]!;
            var jwt = await service.RemakeToken(cookieRefresh,cookieToken);
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
            await service.Logout(refreshToken!);
            Response.Cookies.Delete("Jwt");
            Response.Cookies.Delete("RefreshToken");
            return NoContent();
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
                Secure = false,
            };

            return cookie;
        }
    }
}