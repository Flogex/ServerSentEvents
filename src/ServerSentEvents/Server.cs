using System;
using System.Threading.Tasks;
using ServerSentEvents.Events;

namespace ServerSentEvents
{
    public class Server
    {
        public Task Send(Client client, IEvent @event)
            => @event.WriteToStream(client.HttpContext.Response.Body);

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
