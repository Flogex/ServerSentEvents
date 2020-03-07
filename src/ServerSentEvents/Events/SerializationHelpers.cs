using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ServerSentEvents.Events
{
    internal static class SerializationHelpers
    {
        private static readonly byte[] _linefeed = new byte[] { 10 };

        public static async Task WriteLabeledLines(this Stream stream, byte[] label, string lines)
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

                await stream.WriteAll(label).ConfigureAwait(false);
                var bytes = Encoding.UTF8.GetBytes(lines, startIndex, charsCount);
                await stream.WriteAll(bytes).ConfigureAwait(false);
                await stream.WriteLineFeed().ConfigureAwait(false);

                startIndex = lineFeedIndex + 1;
            }
            while (startIndex < lines.Length);
        }

        public static Task WriteLineFeed(this Stream stream)
            => stream.WriteAsync(_linefeed, 0, _linefeed.Length);

        public static Task WriteAll(this Stream stream, byte[] buffer)
            => stream.WriteAsync(buffer, 0, buffer.Length);
    }
}
