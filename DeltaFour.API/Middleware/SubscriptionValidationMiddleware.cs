using DeltaFour.Domain.Enum;
using DeltaFour.Domain.IRepositories;

namespace DeltaFour.API.Middleware;

public class SubscriptionValidationMiddleware
{
    private readonly RequestDelegate _next;

    private static readonly HashSet<string> ExemptPaths = new(StringComparer.OrdinalIgnoreCase)
    {
        "/api/v1/auth/login",
        "/api/v1/auth/refresh",
        "/api/v1/subscription/register",
        "/api/v1/webhook/subscription",
        "/swagger"
    };

    public SubscriptionValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IUnitOfWork unitOfWork)
    {
        var path = context.Request.Path.Value ?? string.Empty;

        if (ExemptPaths.Any(exemptPath => path.StartsWith(exemptPath, StringComparison.OrdinalIgnoreCase)))
        {
            await _next(context);
            return;
        }

        if (context.User.Identity?.IsAuthenticated == true)
        {
            var companyIdClaim = context.User.FindFirst("CompanyId")?.Value;

            if (Guid.TryParse(companyIdClaim, out var companyId))
            {
                var subscription = await unitOfWork.SubscriptionRepository.Find(s => s.CompanyId == companyId);

                if (subscription != null)
                {
                    if (subscription.Status == SubscriptionStatus.CANCELED.ToString())
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        await context.Response.WriteAsJsonAsync(new
                        {
                            message = "Subscription canceled. Please renew your subscription to continue."
                        });
                        return;
                    }

                    if (subscription.Status == SubscriptionStatus.PAST_DUE.ToString())
                    {
                        context.Response.StatusCode = StatusCodes.Status402PaymentRequired;
                        await context.Response.WriteAsJsonAsync(new
                        {
                            message = "Payment past due. Please update your payment method to continue."
                        });
                        return;
                    }
                }
            }
        }

        await _next(context);
    }
}
