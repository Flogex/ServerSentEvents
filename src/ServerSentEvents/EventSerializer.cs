using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ServerSentEvents
{
    internal static class EventSerializer
    {
        private static readonly byte[] _linefeed = new byte[] { 10 };
        private static readonly byte[] _colon = new byte[] { 58 };
        private static readonly byte[] _eventLabel = Encoding.UTF8.GetBytes("event: ");
        private static readonly byte[] _dataLabel = Encoding.UTF8.GetBytes("data: ");

        public static async Task WriteEvent(Stream stream, Event @event)
        {
            if (@event.Type != null)
                await WriteEventType(stream, @event.Type);

            await WriteEventData(stream, @event.Data);

            await stream.FlushAsync();
        }

        public static async Task WriteComment(Stream stream, string comment)
        {
            await WriteLabeledLines(stream, _colon, comment);
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
            await WriteLabeledLines(stream, _dataLabel, data);
            await stream.WriteLineFeed();
        }

        private static async Task WriteLabeledLines(Stream stream, byte[] label, string lines)
        {
            var startIndex = 0;
            do
            {
                var lineFeedIndex = lines.IndexOf('\n', startIndex);

                if (lineFeedIndex == startIndex) // Ignore empty lines
                {
                    startIndex++;
                    continue;
                }

                if (lineFeedIndex == -1)
                    lineFeedIndex = lines.Length;

                var charsCount = lineFeedIndex - startIndex; // Ignore linefeed

                if (lines[lineFeedIndex - 1] == '\r')
                    charsCount--;

                await stream.WriteAll(label);
                var bytes = Encoding.UTF8.GetBytes(lines, startIndex, charsCount);
                await stream.WriteAll(bytes);
                await stream.WriteLineFeed();

                startIndex = lineFeedIndex + 1;
            }
            while (startIndex < lines.Length);
        }

        private static Task WriteLineFeed(this Stream stream)
            => stream.WriteAsync(_linefeed, 0, _linefeed.Length);

        private static Task WriteAll(this Stream stream, byte[] buffer)
            => stream.WriteAsync(buffer, 0, buffer.Length);
    }
}
