using System;
using System.IO;
using System.Threading.Tasks;

namespace ServerSentEvents.Test.Unit.Fakes
{
    internal class FakeClient : IClient
    {
        public Stream Stream { get; } = new MemoryStream();

        public string LastEventId { get; set; }

        public event EventHandler ConnectionClosed;

        public Task CloseConnection()
        {
            ConnectionClosed?.Invoke(this, EventArgs.Empty);
            return Task.CompletedTask;
        }

        public async Task<string> ReadStreamFromStart()
        {
            Stream.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(Stream);
            var content = await reader.ReadToEndAsync();
            return content;
        }
    }
}
