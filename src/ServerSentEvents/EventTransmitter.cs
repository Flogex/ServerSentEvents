using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ServerSentEvents.Events;

namespace ServerSentEvents
{
    public class EventTransmitter
    {
        public ClientManager Clients { get; } = new ClientManager();

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

        public Task Broadcast(
            IEvent @event,
            CancellationToken cancellationToken = default)
        {
            var clients = Clients.GetAll();
            var tasks = clients.Select(c => Send(c, @event, cancellationToken));
            return Task.WhenAll(tasks);
        }

        public Task BroadcastEvent(
            string data,
            string? type = null,
            string? id = null,
            CancellationToken cancellationToken = default)
            => Broadcast(new Event(data, type, id), cancellationToken);

        public Task BroadcastComment(
            string comment,
            CancellationToken cancellationToken = default)
            => Broadcast(new Comment(comment), cancellationToken);

        public Task BroadcastWaitRequest(
            TimeSpan reconnectionTime,
            CancellationToken cancellationToken = default)
            => Broadcast(new WaitRequest(reconnectionTime), cancellationToken);
    }
}
