using DeltaFour.Domain.Entities;
using DeltaFour.Domain.IRepositories;
using Microsoft.AspNetCore.Http;

namespace DeltaFour.CrossCutting.Middleware
{
    public class UserMiddleware(RequestDelegate next)
    {
        public async Task Invoke(HttpContext context, IEmployeeRepository employeeRepository)
        {
            if (context.User?.Identity?.IsAuthenticated ?? false)
            {
                var userIdClaim = context.User.FindFirst("userId")?.Value;

                if (Guid.TryParse(userIdClaim, out var userId))
                {
                    Employee? user = await employeeRepository.FindForUserAuthenticated(userId);

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
