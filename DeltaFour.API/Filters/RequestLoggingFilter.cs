using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace DeltaFour.API.Filters;

public class RequestLoggingFilter : IAsyncActionFilter
{
    private readonly ILogger<RequestLoggingFilter> _logger;

    public RequestLoggingFilter(ILogger<RequestLoggingFilter> logger)
    {
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;
        var path = httpContext.Request.Path;
        var method = httpContext.Request.Method;

        _logger.LogInformation("Starting request {Method} {Path}", method, path);

        var executedContext = await next();

        if (executedContext.Exception != null && !executedContext.ExceptionHandled)
        {
            _logger.LogError(executedContext.Exception, "Request {Method} {Path} finished with exception: {Message}", method, path, executedContext.Exception.Message);
        }
        else
        {
            var status = httpContext.Response?.StatusCode;
            _logger.LogInformation("Request {Method} {Path} finished with status {Status}", method, path, status);
        }
    }
}
