using Microsoft.AspNetCore.Mvc;
using DeltaFour.Application.Dtos;
using DeltaFour.Application.Service;
using DeltaFour.CrossCutting.Middleware;
using DeltaFour.Domain.Entities;
using Microsoft.AspNetCore.Authorization;

namespace DeltaFour.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(AuthService service) : Controller
    {

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await service.Login(loginDto);
            if (user == null)
            {
                return BadRequest("Ops, Usuario não existe");
            }

            string jwt = service.CreateToken(user);
            Guid refreshToken = await service.CreateRefreshToken(user, jwt);

            CookieOptions options = Cookie();

            Response.Cookies.Append("Jwt", jwt, options);
            Response.Cookies.Append("RefreshToken", refreshToken.ToString(), options);
            return Ok();
        }

        [HttpPost("check-session")]
        [Authorize]
        public IActionResult CheckSession()
        {
            var user = HttpContext.User;
            if (user?.Identity?.IsAuthenticated == true)
            {
                return NoContent();
            }
            return Forbid();
        }

        [HttpPost("refresh-token")]
        [Authorize]
        public async Task<IActionResult> RefreshToken()
        {
            var cookieRefresh = Request.Cookies["RefreshToken"]!;
            var user = HttpContext.GetUserAuthenticated<Employee>();
            var jwt = await service.RemakeToken(cookieRefresh, user.Id.ToString());
            if (jwt != null)
            {
                Response.Cookies.Append("Jwt", jwt, Cookie());
                return NoContent();
            }
            return Forbid();
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["RefreshToken"];
            await service.Logout(refreshToken!);
            Response.Cookies.Delete("Jwt");
            Response.Cookies.Delete("RefreshToken");
            return NoContent();
        }

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