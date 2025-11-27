using DeltaFour.Maui.Services;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DeltaFour.Maui.Handlers
{
    public sealed class CustomAuthHandler : DelegatingHandler
    {
        private const string BypassHeaderName = "X-Bypass-Auth";
        private readonly ISession _session;

        public CustomAuthHandler(ISession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
#if ANDROID
            //Trace.WriteLine($"[AUTH-HANDLER ANDROID] {request.Method} {request.RequestUri}");
#endif
            if (request.Headers.Contains(BypassHeaderName))
                return await SendDownstreamAsync(request, cancellationToken);

            if (IsAuthInfrastructureEndpoint(request.RequestUri))
                return await SendDownstreamAsync(request, cancellationToken);

            var sessionOk = await EnsureSessionAsync(request, cancellationToken);
            if (!sessionOk)
                throw new UnauthorizedAccessException("Sessão expirada ou inválida.");

            var clonedRequest = await CloneHttpRequestMessageAsync(request, cancellationToken);
            var response = await SendDownstreamAsync(request, cancellationToken);

            if (response.StatusCode != HttpStatusCode.Unauthorized)
                return response;

            response.Dispose();

            var refreshed = await RefreshTokenAsync(request, cancellationToken);
            if (!refreshed)
                throw new UnauthorizedAccessException("Não foi possível renovar a sessão.");

            return await SendDownstreamAsync(clonedRequest, cancellationToken);
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

        private async Task<bool> EnsureSessionAsync(
            HttpRequestMessage originalRequest,
            CancellationToken cancellationToken)
        {
            var checkUri = BuildAbsoluteUri(originalRequest, "api/v1/auth/check-session");

            using var request = new HttpRequestMessage(HttpMethod.Get, checkUri);
            request.Headers.Add(BypassHeaderName, "true");

            using var response = await SendDownstreamAsync(request, cancellationToken);

            if (response.IsSuccessStatusCode)
                return true;

            if (response.StatusCode == HttpStatusCode.Forbidden)
                return await RefreshTokenAsync(originalRequest, cancellationToken);

            return false;
        }

        private async Task<bool> RefreshTokenAsync(
            HttpRequestMessage originalRequest,
            CancellationToken cancellationToken)
        {
            var refreshUri = BuildAbsoluteUri(originalRequest, "api/v1/auth/refresh-token");

            using var request = new HttpRequestMessage(HttpMethod.Post, refreshUri);
            request.Headers.Add(BypassHeaderName, "true");

            using var response = await SendDownstreamAsync(request, cancellationToken);
            return response.IsSuccessStatusCode;
        }

        private async Task<HttpResponseMessage> SendDownstreamAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            AddAuthCookiesIfPresent(request);

            var response = await base.SendAsync(request, cancellationToken);

            await CaptureTokensFromSetCookieAsync(response);

            return response;
        }

        private void AddAuthCookiesIfPresent(HttpRequestMessage request)
        {
            //Trace.WriteLine($"[AUTH] AddAuthCookies Jwt={_session.JwtToken} Refresh={_session.RefreshToken}");

            if (string.IsNullOrEmpty(_session.JwtToken) &&
                string.IsNullOrEmpty(_session.RefreshToken))
                return;

            var parts = new System.Collections.Generic.List<string>();

            if (!string.IsNullOrEmpty(_session.JwtToken))
                parts.Add($"Jwt={_session.JwtToken}");

            if (!string.IsNullOrEmpty(_session.RefreshToken))
                parts.Add($"RefreshToken={_session.RefreshToken}");

            request.Headers.Remove("Cookie");
            request.Headers.Add("Cookie", string.Join("; ", parts));
        }

        private async Task CaptureTokensFromSetCookieAsync(HttpResponseMessage response)
        {
            var uri = response.RequestMessage?.RequestUri;
            var path = uri?.AbsolutePath.ToLowerInvariant() ?? string.Empty;

            var isLogin = path.Contains("/api/v1/auth/login");
            var isRefresh = path.Contains("/api/v1/auth/refresh-token");
            var isLogout = path.Contains("/api/v1/auth/logout");

            var canUpdateTokens = isLogin || isRefresh || isLogout;
            if (!canUpdateTokens)
                return;

            if (!response.Headers.TryGetValues("Set-Cookie", out var setCookies))
                return;

            string? newJwt = null;
            string? newRefresh = null;

            foreach (var sc in setCookies)
            {
                var firstPart = sc.Split(';', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                if (string.IsNullOrWhiteSpace(firstPart))
                    continue;

                var kv = firstPart.Split('=', 2);
                if (kv.Length != 2)
                    continue;

                var name = kv[0].Trim();
                var value = kv[1].Trim();

                if (name.Equals("Jwt", StringComparison.OrdinalIgnoreCase))
                    newJwt = value;
                else if (name.Equals("RefreshToken", StringComparison.OrdinalIgnoreCase))
                    newRefresh = value;
            }

            if (!isLogout)
            {
                if (!string.IsNullOrEmpty(newJwt))
                    _session.JwtToken = newJwt;

                if (!string.IsNullOrEmpty(newRefresh))
                    _session.RefreshToken = newRefresh;
            }
            else
            {
                _session.JwtToken = null;
                _session.RefreshToken = null;
            }

            await _session.SaveAsync();
        }

        private static Uri BuildAbsoluteUri(HttpRequestMessage originalRequest, string relativePath)
        {
            if (originalRequest.RequestUri == null || !originalRequest.RequestUri.IsAbsoluteUri)
                throw new ArgumentException("A requisição original precisa ter URI absoluta.", nameof(originalRequest));

            var baseUri = new Uri(originalRequest.RequestUri.GetLeftPart(UriPartial.Authority));
            return new Uri(baseUri, relativePath);
        }

        private static async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
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
