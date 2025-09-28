using DeltaFour.Domain.IRepositories;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DeltaFour.CrossCutting.Middleware
{
    public class UserMiddleware(RequestDelegate next)
    {
        public async Task Invoke(HttpContext context, IUserRepository userRepository)
        {
            if (context.User?.Identity?.IsAuthenticated ?? false)
            {
                var userIdClaim = context.User.FindFirst("userId")?.Value;

                if (Guid.TryParse(userIdClaim, out var userId))
                {
                    var user = await userRepository.GetUserById(userId);

                    if (user != null && user.IsActive)
                    {
                        var claims = new List<Claim>
                        {
                            new Claim("userId", user.Id.ToString()),
                            new Claim("CompanyId", user.CompanyId.ToString()),
                        };

                        var identity = new ClaimsIdentity(claims, "Jwt");
                        context.User = new ClaimsPrincipal(identity);
                    }
                }
            }

            await next(context);
        }
    }
}
