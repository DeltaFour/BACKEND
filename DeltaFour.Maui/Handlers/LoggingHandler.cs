using System.Diagnostics;
using System.Net.Http;
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
            // REQUEST
            Trace.WriteLine("====== HTTP REQUEST ======");
            Trace.WriteLine($"{request.Method} {request.RequestUri}");

            if (request.Content != null)
            {
                var reqBody = await request.Content.ReadAsStringAsync(cancellationToken);
                Trace.WriteLine("Request Body:");
                Trace.WriteLine(reqBody);
            }

            var response = await base.SendAsync(request, cancellationToken);

            // RESPONSE
            Trace.WriteLine("====== HTTP RESPONSE ======");
            Trace.WriteLine($"Status: {(int)response.StatusCode} {response.ReasonPhrase}");

            if (response.Content != null)
            {
                var respBody = await response.Content.ReadAsStringAsync(cancellationToken);
                Trace.WriteLine("Response Body:");
                Trace.WriteLine(respBody);
            }

            Trace.WriteLine("===========================\n");

            return response;
        }
    }
}
