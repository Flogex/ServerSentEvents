using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ServerSentEvents.Events;

namespace ServerSentEvents
{
    public class Server
    {
        private readonly Dictionary<ClientId, HttpContext> _clients = new Dictionary<ClientId, HttpContext>();

        public async Task<ClientId> AddClient(HttpContext context)
        {
            var id = ClientId.NewClientId();
            _clients.Add(id, context);

            context.RequestAborted.Register(state => RemoveClient((ClientId)state!), id);

            var response = context.Response;
            response.StatusCode = 200;
            response.ContentType = "text/event-stream";
            response.Headers.Add("Cache-Control", "no-cache");
            await response.StartAsync();

            return id;
        }

        public void RemoveClient(ClientId id)
        {
            if (_clients.Remove(id, out var context))
                context.Abort();
        }

        public Task Send(ClientId clientId, IEvent @event)
        {
            if (!_clients.TryGetValue(clientId, out var client))
            {
                var message = $"Unknown client with id {clientId}.";
                throw new ArgumentException(message, nameof(clientId));
            }

            return @event.WriteToStream(client.Response.Body);
        }

        public Task SendEvent(
            ClientId clientId,
            string data,
            string? type = null,
            string? id = null)
            => Send(clientId, new Event(data, type, id));

        public Task SendComment(ClientId clientId, string comment)
            => Send(clientId, new Comment(comment));

        public Task SendWaitRequest(ClientId clientId, TimeSpan reconnectionTime)
            => Send(clientId, new WaitRequest(reconnectionTime));
    }
}
