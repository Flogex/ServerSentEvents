using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace ServerSentEvents
{
    public partial class HttpClient : IClient
    {
        private readonly Guid _id = Guid.NewGuid();

        private HttpClient(HttpContext context)
        {
            HttpContext = context;

            var headers = HttpContext.Request?.Headers;
            if (headers?.TryGetValue("Last-Event-Id", out var lastEventId) == true)
                LastEventId = lastEventId;

            HttpContext.RequestAborted.Register(OnConnectionClosed);
        }

        internal HttpContext HttpContext { get; }

        public Stream Stream => HttpContext.Response.Body;

        public string? LastEventId { get; }

        public event EventHandler? ConnectionClosed;

        private void OnConnectionClosed()
            => ConnectionClosed?.Invoke(this, EventArgs.Empty);

        // OnConnectionClosed gets called because we registered this method
        // on 'HttpContext.RequestAborted' CancellationToken
        public async Task CloseConnection()
        {
            await HttpContext.Response.CompleteAsync().ConfigureAwait(false);
            HttpContext.Abort();
        }

        public static async Task<HttpClient> NewClient(HttpContext context)
        {
            if (context.Response.HasStarted)
                throw new ArgumentException("HttpContext.Response has already started.",
                    nameof(context));

            await PrepareHttpResponse(context.Response).ConfigureAwait(false);

            return new HttpClient(context);
        }

        private static Task PrepareHttpResponse(HttpResponse response)
        {
            response.StatusCode = Status200OK;
            response.ContentType = "text/event-stream";
            response.Headers.Add("Cache-Control", "no-cache");
            response.Headers.Add("Transfer-Encoding", "identity");
            response.Headers.Add("Connection", "keep-alive");
            return response.StartAsync();
        }

        public static Task StopReconnecting(HttpContext context)
        {
            var response = context.Response;
            response.StatusCode = Status204NoContent;
            response.ContentType = "text/event-stream";
            response.ContentLength = 0;
            response.Headers.Add("Connection", "close");
            return response.CompleteAsync();
        }
    }
}
