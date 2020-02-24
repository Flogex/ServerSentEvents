using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ServerSentEvents
{
    internal static class EventSerializer
    {
        private static readonly byte[] _linefeed = new byte[] { 10 };
        private static readonly byte[] _colon = new byte[] { 58 };
        private static readonly byte[] _idLabel = Encoding.UTF8.GetBytes("id:");
        private static readonly byte[] _eventLabel = Encoding.UTF8.GetBytes("event:");
        private static readonly byte[] _dataLabel = Encoding.UTF8.GetBytes("data:");
        private static readonly byte[] _retryLabel = Encoding.UTF8.GetBytes("retry:");

        public static async Task WriteEvent(Stream stream, Event @event)
        {
            if (@event.Id != null)
                await stream.WriteEventId(@event.Id);

            if (@event.Type != null)
                await stream.WriteEventType(@event.Type);

            await stream.WriteEventData(@event.Data);

            await stream.FlushAsync();
        }

        public static async Task WriteComment(Stream stream, string comment)
        {
            await stream.WriteLabeledLines(_colon, comment);
            await stream.WriteLineFeed();
            await stream.FlushAsync();
        }

        public static async Task WriteRetry(Stream stream, TimeSpan reconnectionTime)
        {
            await stream.WriteAll(_retryLabel);

            var milliseconds = (int)Math.Round(reconnectionTime.TotalMilliseconds,
                                               0, MidpointRounding.AwayFromZero);
            var bytes = milliseconds.ToByteArray();

            await stream.WriteAll(bytes);
            await stream.WriteLineFeed();
            await stream.WriteLineFeed();
            await stream.FlushAsync();
        }

        private static async Task WriteEventId(this Stream stream, string id)
        {
            await stream.WriteAll(_idLabel);
            var bytes = Encoding.UTF8.GetBytes(id);
            await stream.WriteAll(bytes);
            await stream.WriteLineFeed();
        }

        private static async Task WriteEventType(this Stream stream, string type)
        {
            await stream.WriteAll(_eventLabel);
            var bytes = Encoding.UTF8.GetBytes(type);
            await stream.WriteAll(bytes);
            await stream.WriteLineFeed();
        }

        private static async Task WriteEventData(this Stream stream, string data)
        {
            await stream.WriteLabeledLines(_dataLabel, data);
            await stream.WriteLineFeed();
        }

        private static async Task WriteLabeledLines(this Stream stream, byte[] label, string lines)
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
