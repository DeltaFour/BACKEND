using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DeltaFour.API.Filters;

/// <summary>
/// Autoriza a requisição via API-key (comunicação máquina-a-máquina).
/// A chave deve ser enviada no header <c>X-API-Key</c> e precisa bater com
/// a variável de ambiente <c>KMEANS_API_KEY</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public sealed class ApiKeyAuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
{
    public const string HeaderName = "X-API-Key";
    private const string EnvVarName = "KMEANS_API_KEY";

    public Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var configuredKey = Environment.GetEnvironmentVariable(EnvVarName);

        if (string.IsNullOrWhiteSpace(configuredKey))
        {
            // Chave não configurada no servidor: nega por segurança.
            context.Result = new ObjectResult(new { message = "API key authentication is not configured." })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
            return Task.CompletedTask;
        }

        if (!context.HttpContext.Request.Headers.TryGetValue(HeaderName, out var providedKey)
            || string.IsNullOrWhiteSpace(providedKey)
            || !FixedTimeEquals(providedKey.ToString(), configuredKey))
        {
            context.Result = new UnauthorizedObjectResult(new { message = "Invalid or missing API key." });
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Comparação em tempo constante para evitar timing attacks.
    /// </summary>
    private static bool FixedTimeEquals(string a, string b)
    {
        var bytesA = System.Text.Encoding.UTF8.GetBytes(a);
        var bytesB = System.Text.Encoding.UTF8.GetBytes(b);
        return System.Security.Cryptography.CryptographicOperations.FixedTimeEquals(bytesA, bytesB);
    }
}
