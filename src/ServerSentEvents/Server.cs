using System;
using System.Threading.Tasks;
using ServerSentEvents.Events;

namespace ServerSentEvents
{
    public class Server
    {
        public Task Send(HttpClient client, IEvent @event)
            => @event.WriteToStream(client.Stream);

        public Task SendEvent(
            HttpClient client,
            string data,
            string? type = null,
            string? id = null)
            => Send(client, new Event(data, type, id));

        public Task SendComment(HttpClient client, string comment)
            => Send(client, new Comment(comment));

        public Task SendWaitRequest(HttpClient client, TimeSpan reconnectionTime)
            => Send(client, new WaitRequest(reconnectionTime));
    }
}
