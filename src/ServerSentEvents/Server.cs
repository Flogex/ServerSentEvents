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

        public Task Send(ClientId id, IEvent @event)
        {
            if (!_clients.TryGetValue(id, out var client))
                throw new ArgumentException($"Unknown client with id {id}.", nameof(id));

            return @event.WriteToStream(client.Response.Body);
        }

        public Task SendEvent(ClientId id, Event @event)
            => Send(id, @event);

        public Task SendComment(ClientId id, string comment)
            => Send(id, new Comment(comment));

        public Task SendWaitRequest(ClientId id, TimeSpan reconnectionTime)
            => Send(id, new WaitRequest(reconnectionTime));
    }
}
