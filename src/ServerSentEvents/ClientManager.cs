using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ServerSentEvents
{
    public class ClientManager
    {
        private readonly List<IClient> _clients = new List<IClient>();

        internal event EventHandler<ClientAddedEventArgs>? ClientAdded;

        public IReadOnlyList<IClient> GetAll()
            => new ReadOnlyCollection<IClient>(_clients);

        public void Add(IClient client)
        {
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
