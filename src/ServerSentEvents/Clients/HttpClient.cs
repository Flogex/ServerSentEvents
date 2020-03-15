using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ServerSentEvents
{
    public partial class HttpClient : IClient, IEquatable<HttpClient>
    {
        private readonly Guid _id = Guid.NewGuid();

        private HttpClient(HttpContext httpContext)
        {
            HttpContext = httpContext;

            var headers = httpContext.Request?.Headers;
            if (headers?.TryGetValue("Last-Event-Id", out var lastEventId) == true)
                LastEventId = lastEventId;

            httpContext.RequestAborted.Register(OnConnectionClosed);
        }

        internal HttpContext HttpContext { get; }

        public Stream Stream => HttpContext.Response.Body;

        public string? LastEventId { get; }

        public event EventHandler? ConnectionClosed;

        private void OnConnectionClosed()
            => ConnectionClosed?.Invoke(this, EventArgs.Empty);

        // OnConnectionClosed called because we registered method
        // on RequestAborted CancellationToken
        public void CloseConnection() => HttpContext.Abort();

        public static async Task<HttpClient> NewClient(HttpContext httpContext)
        {
            if (httpContext.Response.HasStarted)
                throw new InvalidOperationException("Response has already started.");

            await PrepareHttpResponse(httpContext.Response).ConfigureAwait(false);

            return new HttpClient(httpContext);
        }

        private static Task PrepareHttpResponse(HttpResponse response)
        {
            response.StatusCode = 200;
            response.ContentType = "text/event-stream";
            response.Headers.Add("Cache-Control", "no-cache");
            return response.StartAsync();
        }
    }
}
