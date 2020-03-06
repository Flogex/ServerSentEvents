using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ServerSentEvents
{
    public partial class Client : IEquatable<Client>
    {
        private readonly Guid _id = Guid.NewGuid();

        private Client(HttpContext httpContext)
        {
            HttpContext = httpContext;
        }

        internal HttpContext HttpContext { get; }

        public static async Task<Client> NewClient(HttpContext httpContext)
        {
            if (httpContext.Response.HasStarted)
                throw new InvalidOperationException();

            await PrepareHttpResponse(httpContext.Response);

            return new Client(httpContext);
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
