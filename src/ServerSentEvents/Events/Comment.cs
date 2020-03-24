using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ServerSentEvents.Events
{
    public class Comment : IEvent
    {
        private static readonly byte[] _colon = new byte[] { 58 };

        public Comment(string comment)
        {
            Value = comment ?? throw new ArgumentNullException(nameof(comment));
        }

        public string Value { get; }

        public async Task WriteToStream(
            Stream stream,
            CancellationToken cancellationToken = default)
        {
            await stream.WriteLabeledLines(_colon, Value).ConfigureAwait(false);
            await stream.WriteLineFeed().ConfigureAwait(false);
        }
    }
}
