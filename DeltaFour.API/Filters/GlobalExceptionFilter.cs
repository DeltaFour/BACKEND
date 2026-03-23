using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace DeltaFour.API.Filters;

public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger, IHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "An unhandled exception occurred: {Message}", context.Exception.Message);

        var response = CreateErrorResponse(context.Exception);

        context.Result = new ObjectResult(response)
        {
            StatusCode = response.StatusCode
        };

        context.ExceptionHandled = true;
    }

    private ErrorResponse CreateErrorResponse(Exception exception)
    {
        var (statusCode, message) = exception switch
        {
            ArgumentNullException => (HttpStatusCode.BadRequest, "Um ou mais parâmetros obrigatórios não foram fornecidos."),
            ArgumentException => (HttpStatusCode.BadRequest, exception.Message),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Acesso não autorizado."),
            KeyNotFoundException => (HttpStatusCode.NotFound, "Recurso não encontrado."),
            InvalidOperationException => (HttpStatusCode.BadRequest, exception.Message),
            _ => (HttpStatusCode.InternalServerError, "Ocorreu um erro interno no servidor.")
        };

        var response = new ErrorResponse
        {
            StatusCode = (int)statusCode,
            Message = message,
            Timestamp = DateTime.UtcNow
        };

        if (_environment.IsDevelopment())
        {
            response.Details = exception.Message;
            response.StackTrace = exception.StackTrace;
        }

        return response;
    }
}

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? Details { get; set; }
    public string? StackTrace { get; set; }
}
