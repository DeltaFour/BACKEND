using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DeltaFour.Maui.Handlers
{
    public sealed class LoggingHandler : DelegatingHandler
    {
        public LoggingHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            Trace.WriteLine("====== HTTP REQUEST ======");
            Trace.WriteLine($"{request.Method} {request.RequestUri}");

            if (request.Content != null)
            {
                var reqBody = await TryReadBodyAsync(request.Content, "request", cancellationToken);
                if (reqBody != null)
                {
                    Trace.WriteLine("Request Body:");
                    Trace.WriteLine(reqBody);
                    request.Content = CloneTextContent(reqBody, request.Content);
                }
            }

            var response = await base.SendAsync(request, cancellationToken);

            Trace.WriteLine("====== HTTP RESPONSE ======");
            Trace.WriteLine($"Status: {(int)response.StatusCode} {response.ReasonPhrase}");

            if (response.Content != null)
            {
                var respBody = await TryReadBodyAsync(response.Content, "response", cancellationToken);
                if (respBody != null)
                {
                    Trace.WriteLine("Response Body:");
                    Trace.WriteLine(respBody);
                    response.Content = CloneTextContent(respBody, response.Content);
                }
            }

            Trace.WriteLine("===========================\n");

            return response;
        }

        private static async Task<string?> TryReadBodyAsync(
            HttpContent content,
            string phase,
            CancellationToken cancellationToken)
        {
            try
            {
                return await content.ReadAsStringAsync(cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex) when (ex is ObjectDisposedException
                                       || ex is IOException
                                       || ex is HttpRequestException
                                       || ex is InvalidOperationException)
            {
                Trace.WriteLine($"[HTTP LOG] {phase} body unavailable: {ex.GetType().Name}: {ex.Message}");
                return null;
            }
        }

        private static HttpContent CloneTextContent(string body, HttpContent originalContent)
        {
            var encoding = GetContentEncoding(originalContent);
            var clone = new ByteArrayContent(encoding.GetBytes(body));

            foreach (var header in originalContent.Headers)
            {
                if (header.Key.Equals("Content-Length", StringComparison.OrdinalIgnoreCase))
                    continue;

                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            return clone;
        }

        private static Encoding GetContentEncoding(HttpContent content)
        {
            var charset = content.Headers.ContentType?.CharSet?.Trim('"');
            if (string.IsNullOrWhiteSpace(charset))
                return Encoding.UTF8;

            try
            {
                return Encoding.GetEncoding(charset);
            }
            catch (ArgumentException)
            {
                return Encoding.UTF8;
            }
        }
    }
}
