using System;
using System.IO;
using System.Threading.Tasks;

namespace ServerSentEvents.Events
{
    public class Comment : IEvent
    {
        private static readonly byte[] _colon = new byte[] { 58 };

        public Comment(string value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string Value { get; }

        public async Task WriteToStream(Stream stream)
        {
            await stream.WriteLabeledLines(_colon, Value).ConfigureAwait(false);
            await stream.WriteLineFeed().ConfigureAwait(false);
        }
    }
}
