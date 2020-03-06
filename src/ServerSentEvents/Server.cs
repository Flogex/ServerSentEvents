using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ServerSentEvents.Events;

namespace ServerSentEvents
{
    public class Server
    {
        private readonly Dictionary<Client, HttpContext> _clients = new Dictionary<Client, HttpContext>();

        public async Task<Client> AddClient(HttpContext context)
        {
            var client = Client.NewClient();
            _clients.Add(client, context);

            context.RequestAborted.Register(state => RemoveClient((Client)state!), client);

            await PrepareHttpResponse(context.Response);

            return client;
        }

        private static async Task PrepareHttpResponse(HttpResponse response)
        {
            response.StatusCode = 200;
            response.ContentType = "text/event-stream";
            response.Headers.Add("Cache-Control", "no-cache");
            await response.StartAsync();
        }

        public void RemoveClient(Client client)
        {
            if (_clients.Remove(client, out var context))
                context.Abort();
        }

        public Task Send(Client client, IEvent @event)
        {
            if (!_clients.TryGetValue(client, out var context))
            {
                var message = $"Unknown client with id { client.Id }.";
                throw new ArgumentException(message, nameof(client));
            }

            return @event.WriteToStream(context.Response.Body);
        }

        public Task SendEvent(
            Client client,
            string data,
            string? type = null,
            string? id = null)
            => Send(client, new Event(data, type, id));

        public Task SendComment(Client client, string comment)
            => Send(client, new Comment(comment));

        public Task SendWaitRequest(Client client, TimeSpan reconnectionTime)
            => Send(client, new WaitRequest(reconnectionTime));
    }
}
