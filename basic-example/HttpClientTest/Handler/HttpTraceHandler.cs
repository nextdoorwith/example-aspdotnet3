using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpClientTest.Handler
{
    public class HttpTraceHandler : DelegatingHandler
    {
        private readonly ILogger<HttpTraceHandler> _logger;

        public HttpTraceHandler(ILogger<HttpTraceHandler> logger)
        {
            _logger = logger;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {

            // HTTPリクエスト情報のダンプ
            var reqsb = new StringBuilder();
            reqsb.AppendLine($"{request.Method} {request.RequestUri.LocalPath} HTTP/{request.Version}");
            foreach (var header in request.Headers) {
                reqsb.AppendLine($"{header.Key}: {string.Join(", ", header.Value)}");
            };
            reqsb.AppendLine();
            HttpContent content = request.Content;
            if( content != null)
            {
                Task<string> requestBody = content.ReadAsStringAsync();
                reqsb.AppendLine(requestBody.Result);
            }

            _logger.LogDebug("Request==========\n" + reqsb);

            // HTTPレスポンス情報のダンプ
            Task<HttpResponseMessage> response = base.SendAsync(request, cancellationToken);
            var ressb = new StringBuilder();
            ressb.AppendLine($"HTTP/1.1 {response.Status}");
            foreach (var header in response.Result.Content.Headers)
            {
                ressb.AppendLine($"{header.Key}: {string.Join(", ", header.Value)}");
            };
            ressb.AppendLine();
            Task<string> responseBody = response.Result.Content.ReadAsStringAsync();
            ressb.AppendLine(responseBody.Result);

            _logger.LogDebug("Response==========\n" + ressb);

            return response;
        }
    }
}
