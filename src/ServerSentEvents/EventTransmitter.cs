using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ServerSentEvents.Events;

namespace ServerSentEvents
{
    public class EventTransmitter
    {
        private bool _resendEvents;
        private EventHistory? _eventHistory;

        public ClientManager Clients { get; } = new ClientManager();

        public void EnableResend(int maxStoredEvents)
        {
            _eventHistory = new EventHistory(maxStoredEvents);
            Clients.ClientAdded += ResendEvents;
            _resendEvents = true;
        }

        public void DisableResend()
        {
            _resendEvents = false;
            _eventHistory?.Clear();
            _eventHistory = null;
            Clients.ClientAdded -= ResendEvents;
        }

        private async void ResendEvents(object? sender, ClientAddedEventArgs args)
        {
            var client = args.NewClient;
            var eventId = client.LastEventId;

            if (eventId != null)
            {
                var eventsToResend = _eventHistory!.GetSubsequentEvents(eventId);

                foreach (var @event in eventsToResend)
                    await Send(client, @event);
            }
        }

        public async Task Send(
            IClient client,
            IEvent @event,
            CancellationToken cancellationToken = default)
        {
            var stream = client.Stream;

            await @event.WriteToStream(stream, cancellationToken)
                .ConfigureAwait(false);

            await stream.FlushAsync(cancellationToken)
                .ConfigureAwait(false);
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
        {
            var @event = new Event(data, type, id);

            if (_resendEvents)
                _eventHistory?.Add(@event);

            return Broadcast(@event, cancellationToken);
        }

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
