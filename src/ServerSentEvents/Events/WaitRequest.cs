using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerSentEvents.Events
{
    public class WaitRequest : IEvent
    {
        private static readonly byte[] _retryLabel = Encoding.UTF8.GetBytes("retry:");

        public WaitRequest(TimeSpan reconnectionTime)
        {
            if (reconnectionTime < TimeSpan.Zero)
            {
                throw new ArgumentException(
                    "reconnectionTime must be greater or equal zero",
                    nameof(reconnectionTime));
            }

            ReconnectionTime = reconnectionTime;
        }

        public TimeSpan ReconnectionTime { get; }

        public async Task WriteToStream(
            Stream stream,
            CancellationToken cancellationToken = default)
        {
            await stream.WriteAll(_retryLabel).ConfigureAwait(false);

            var milliseconds = GetRoundedMilliseconds(ReconnectionTime);
            var bytes = milliseconds.ToByteArray();

            await stream.WriteAll(bytes).ConfigureAwait(false);
            await stream.WriteLineFeed().ConfigureAwait(false);
            await stream.WriteLineFeed().ConfigureAwait(false);
        }

        private static int GetRoundedMilliseconds(TimeSpan timeSpan)
            => (int)Math.Round(timeSpan.TotalMilliseconds, 0,
                               MidpointRounding.AwayFromZero);
    }
}
