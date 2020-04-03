using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace ServerSentEvents
{
    public partial class ClientManager
    {
        private readonly HashSet<IClient> _clients = new HashSet<IClient>();

        internal event EventHandler<ClientAddedEventArgs>? ClientAdded;

        public IReadOnlyList<IClient> GetAll()
            => new ReadOnlyCollection<IClient>(_clients.ToImmutableList());

        public void Add(IClient client)
        {
            if (client is null)
                throw new ArgumentNullException(nameof(client));

            client.ConnectionClosed += HandleClientConnectionClosed;
            _clients.Add(client);

            ClientAdded?.Invoke(this, new ClientAddedEventArgs(client));
        }

        public void AddRange(IEnumerable<IClient> clients)
        {
            foreach (var client in clients)
                Add(client);
        }

        private void HandleClientConnectionClosed(object? sender, EventArgs _)
        {
            if (sender is IClient client)
                Remove(client);
        }

        public void Remove(IClient client) => _clients.Remove(client);
    }
}
