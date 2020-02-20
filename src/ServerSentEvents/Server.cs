using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ServerSentEvents
{
    public class Server
    {
        public async Task AddClient(HttpResponse response)
        {
            response.StatusCode = 200;
            response.Headers.Add("Cache-Control", "no-cache");
            response.ContentType = "text/event-stream";
            await response.StartAsync();
        }
    }
}
