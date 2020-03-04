using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ServerSentEvents.Events
{
    public class WaitRequest : IEvent
    {
        private static readonly byte[] _retryLabel = Encoding.UTF8.GetBytes("retry:");

        public WaitRequest(TimeSpan reconnectionTime)
        {
            ReconnectionTime = reconnectionTime;
        }

        public TimeSpan ReconnectionTime { get; }

        public async Task WriteToStream(Stream stream)
        {
            await stream.WriteAll(_retryLabel);

            var milliseconds = GetRoundedMilliseconds(ReconnectionTime);
            var bytes = milliseconds.ToByteArray();

            await stream.WriteAll(bytes);
            await stream.WriteLineFeed();
            await stream.WriteLineFeed();
            await stream.FlushAsync();
        }

        private static int GetRoundedMilliseconds(TimeSpan timeSpan)
            => (int)Math.Round(timeSpan.TotalMilliseconds, 0,
                               MidpointRounding.AwayFromZero);
    }
}
