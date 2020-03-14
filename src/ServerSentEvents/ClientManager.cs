using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ServerSentEvents
{
    public class ClientManager
    {
        private readonly List<IClient> _clients = new List<IClient>();

        public IReadOnlyCollection<IClient> Clients
            => new ReadOnlyCollection<IClient>(_clients);

        public void Add(IClient client) => _clients.Add(client);

        public void Remove(IClient client) => _clients.Remove(client);
    }
}
