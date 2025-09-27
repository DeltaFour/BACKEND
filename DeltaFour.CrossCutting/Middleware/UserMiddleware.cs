using DeltaFour.Domain.Entities;
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
                    Employee? user = await userRepository.Find(u => u.Id == userId);

                    if (user is { IsActive: true })
                    {
                        context.SetObject("user", user);
                    }
                }
            }

            await next(context);
        }
    }
}
