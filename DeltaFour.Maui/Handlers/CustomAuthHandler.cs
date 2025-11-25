using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace DeltaFour.Maui.Handlers
{
    public sealed class CustomAuthHandler : DelegatingHandler
    {
        private const string BypassHeaderName = "X-Bypass-Auth";

        public CustomAuthHandler()
        {
        }

        public CustomAuthHandler(HttpMessageHandler innerHandler) : base(innerHandler)
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
#if ANDROID
            Trace.WriteLine($"[AUTH-HANDLER ANDROID] {request.Method} {request.RequestUri}");
#endif
            if (request.Headers.Contains(BypassHeaderName))
                return await base.SendAsync(request, cancellationToken);

            if (IsAuthInfrastructureEndpoint(request.RequestUri))
                return await base.SendAsync(request, cancellationToken);

            var sessionOk = await EnsureSessionAsync(request, cancellationToken);
            if (!sessionOk)
                throw new UnauthorizedAccessException("Sessão expirada ou inválida.");

            var clonedRequest = await CloneHttpRequestMessageAsync(request, cancellationToken);
            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode != HttpStatusCode.Unauthorized)
                return response;

            response.Dispose();

            var refreshed = await RefreshTokenAsync(request, cancellationToken);
            if (!refreshed)
                throw new UnauthorizedAccessException("Não foi possível renovar a sessão.");

            return await base.SendAsync(clonedRequest, cancellationToken);
        }

        private static bool IsAuthInfrastructureEndpoint(Uri? uri)
        {
            if (uri == null)
                return false;

            var path = uri.AbsolutePath.ToLowerInvariant();
            return path.Contains("/api/v1/auth/login")
                   || path.Contains("/api/v1/auth/check-session")
                   || path.Contains("/api/v1/auth/refresh-token") 
                   || path.Contains("/api/v1/auth/logout");
        }

        private async Task<bool> EnsureSessionAsync(HttpRequestMessage originalRequest, CancellationToken cancellationToken)
        {
            var checkUri = BuildAbsoluteUri(originalRequest, "api/v1/auth/check-session");

            using var request = new HttpRequestMessage(HttpMethod.Get, checkUri);
            request.Headers.Add(BypassHeaderName, "true");

            using var response = await base.SendAsync(request, cancellationToken);

            if (response.IsSuccessStatusCode)
                return true;

            if (response.StatusCode == HttpStatusCode.Forbidden)
                return await RefreshTokenAsync(originalRequest, cancellationToken);

            return false;
        }

        private async Task<bool> RefreshTokenAsync(HttpRequestMessage originalRequest, CancellationToken cancellationToken)
        {
            var refreshUri = BuildAbsoluteUri(originalRequest, "api/v1/auth/refresh-token");

            using var request = new HttpRequestMessage(HttpMethod.Post, refreshUri);
            request.Headers.Add(BypassHeaderName, "true");

            using var response = await base.SendAsync(request, cancellationToken);
            return response.IsSuccessStatusCode;
        }

        private static Uri BuildAbsoluteUri(HttpRequestMessage originalRequest, string relativePath)
        {
            if (originalRequest.RequestUri == null || !originalRequest.RequestUri.IsAbsoluteUri)
                throw new ArgumentException("A requisição original precisa ter URI absoluta.", nameof(originalRequest));

            var baseUri = new Uri(originalRequest.RequestUri.GetLeftPart(UriPartial.Authority));
            return new Uri(baseUri, relativePath);
        }

        private static async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var clone = new HttpRequestMessage(request.Method, request.RequestUri);

            foreach (var header in request.Headers)
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

            if (request.Content != null)
            {
                var body = await request.Content.ReadAsStringAsync(cancellationToken);
                var mediaType = request.Content.Headers.ContentType?.MediaType ?? "application/json";
                clone.Content = new StringContent(body, Encoding.UTF8, mediaType);

                foreach (var header in request.Content.Headers)
                {
                    if (header.Key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))
                        continue;

                    clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            clone.Version = request.Version;
            return clone;
        }
    }
}
