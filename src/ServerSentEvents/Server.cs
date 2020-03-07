using System;
using System.Threading.Tasks;
using ServerSentEvents.Events;

namespace ServerSentEvents
{
    public class Server
    {
        public async Task Send(IClient client, IEvent @event)
        {
            var stream = client.Stream;
            await @event.WriteToStream(stream).ConfigureAwait(false);
            await stream.FlushAsync().ConfigureAwait(false);
        }

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
