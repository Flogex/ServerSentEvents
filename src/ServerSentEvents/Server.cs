using System;
using System.Threading.Tasks;
using ServerSentEvents.Events;

namespace ServerSentEvents
{
    public class Server
    {
        public Task Send(IClient client, IEvent @event)
            => @event.WriteToStream(client.Stream);

        public Task SendEvent(
            IClient client,
            string data,
            string? type = null,
            string? id = null)
            => Send(client, new Event(data, type, id));

        public Task SendComment(IClient client, string comment)
            => Send(client, new Comment(comment));

        public Task SendWaitRequest(IClient client, TimeSpan reconnectionTime)
            => Send(client, new WaitRequest(reconnectionTime));
    }
}
