using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ServerSentEvents
{
    internal static class EventSerializer
    {
        private static readonly byte[] _linefeed = new byte[] { 10 };
        private static readonly byte[] _eventLabel = Encoding.UTF8.GetBytes("event: ");
        private static readonly byte[] _dataLabel = Encoding.UTF8.GetBytes("data: ");

        public static async Task WriteEvent(Event @event, Stream stream)
        {
            if (@event.Type != null)
                await WriteEventType(stream, @event.Type);

            await WriteEventData(stream, @event.Data);

            await stream.FlushAsync();
        }

        private static async Task WriteEventType(Stream stream, string type)
        {
            await stream.WriteAsync(_eventLabel, 0, _eventLabel.Length);
            var bytes = Encoding.UTF8.GetBytes(type);
            await stream.WriteAsync(bytes, 0, bytes.Length);
            await stream.WriteLineFeed();
        }

        private static async Task WriteEventData(Stream stream, string data)
        {
            var startIndex = 0;
            do
            {
                var lineFeedIndex = data.IndexOf('\n', startIndex);

                if (lineFeedIndex == startIndex)
                {
                    startIndex++;
                    continue;
                }

                if (lineFeedIndex == -1)
                    lineFeedIndex = data.Length;

                var charsCount = lineFeedIndex - startIndex; // Ignore linefeed

                if (data[lineFeedIndex - 1] == '\r')
                    charsCount--;

                await stream.WriteAsync(_dataLabel, 0, _dataLabel.Length);
                var bytes = Encoding.UTF8.GetBytes(data, startIndex, charsCount);
                await stream.WriteAsync(bytes, 0, bytes.Length);
                await stream.WriteLineFeed();

                startIndex = lineFeedIndex + 1;
            }
            while (startIndex < data.Length);

            await stream.WriteLineFeed();
        }

        private static Task WriteLineFeed(this Stream stream)
            => stream.WriteAsync(_linefeed, 0, _linefeed.Length);
    }
}
