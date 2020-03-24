using System;
using System.IO;
using System.Threading.Tasks;
using ServerSentEvents.Test.Unit.Helpers;

namespace ServerSentEvents.Test.Unit.Fakes
{
    internal class FakeClient : IClient
    {
        public Stream Stream { get; set; } = new MemoryStream();

        public string LastEventId { get; set; }

        public event EventHandler ConnectionClosed;

        public Task CloseConnection()
        {
            ConnectionClosed?.Invoke(this, EventArgs.Empty);
            return Task.CompletedTask;
        }

        public Task<string> ReadStreamFromStart() => Stream.ReadFromStart();
    }
}
