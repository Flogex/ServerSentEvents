using System;
using System.Threading;
using System.Threading.Tasks;
using ServerSentEvents.Events;

namespace ServerSentEvents
{
    public class Server
    {
        public async Task Send(
            IClient client,
            IEvent @event,
            CancellationToken cancellationToken = default)
        {
            var stream = client.Stream;
            await @event.WriteToStream(stream, cancellationToken).ConfigureAwait(false);
            await stream.FlushAsync(cancellationToken).ConfigureAwait(false);
        }

        public Task SendEvent(
            IClient client,
            string data,
            string? type = null,
            string? id = null,
            CancellationToken cancellationToken = default)
            => Send(client, new Event(data, type, id), cancellationToken);

        public Task SendComment(
            IClient client,
            string comment,
            CancellationToken cancellationToken = default)
            => Send(client, new Comment(comment), cancellationToken);

        public Task SendWaitRequest(
            IClient client,
            TimeSpan reconnectionTime,
            CancellationToken cancellationToken = default)
            => Send(client, new WaitRequest(reconnectionTime), cancellationToken);
    }
}
