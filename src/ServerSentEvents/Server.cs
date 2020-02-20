using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ServerSentEvents
{
    public class Server
    {
        private readonly Dictionary<ClientId, HttpResponse> _clients = new Dictionary<ClientId, HttpResponse>();

        public async Task<ClientId> AddClient(HttpResponse response)
        {
            var id = ClientId.NewClientId();
            _clients.Add(id, response);

            response.StatusCode = 200;
            response.Headers.Add("Cache-Control", "no-cache");
            response.ContentType = "text/event-stream";
            await response.StartAsync();

            return id;
        }

        public async Task SendMessage(ClientId id, Event @event)
        {
            if (!_clients.TryGetValue(id, out var client))
                throw new ArgumentException($"Unknown client with id {id}.");

            if (@event.Type != null)
            {
                await client.WriteAsync("event: ");
                await client.WriteAsync(@event.Type);
                await client.WriteAsync("\n");
            }

            await client.WriteAsync("data: ");
            await client.WriteAsync(@event.Data);
            await client.WriteAsync("\n\n");
        }
    }
}
