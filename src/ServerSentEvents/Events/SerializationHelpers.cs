using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ServerSentEvents.Events
{
    internal static class SerializationHelpers
    {
        private const byte LF = 10;
        private const byte CR = 13;

        private static readonly byte[] _linefeed = new byte[] { LF };

        public static async Task WriteLabeledLines(this Stream stream, byte[] label, string lines)
        {
            var bytes = Encoding.UTF8.GetBytes(lines);
            var startIndex = 0;

            do
            {
                var lineFeedIndex = Array.IndexOf(bytes, LF, startIndex);

                // Ignore empty lines
                if (lineFeedIndex == startIndex)
                {
                    startIndex++;
                    continue;
                }

                // Write all remaining bytes
                if (lineFeedIndex == -1)
                    lineFeedIndex = bytes.Length;

                // Do not count linefeed. It is sent with WriteLineFeed().
                var charsCount = lineFeedIndex - startIndex;

                // Ignore carriage returns at the end of a line,
                // except when it is the only byte sent.
                if (bytes[lineFeedIndex - 1] == CR && charsCount > 1)
                    charsCount--;

                await stream.WriteAll(label).ConfigureAwait(false);

                await stream.WriteAsync(bytes, startIndex, charsCount)
                    .ConfigureAwait(false);

                await stream.WriteLineFeed().ConfigureAwait(false);

                startIndex = lineFeedIndex + 1;
            }
            while (startIndex < bytes.Length);
        }

        public static Task WriteLineFeed(this Stream stream)
            => stream.WriteAsync(_linefeed, 0, _linefeed.Length);

        public static Task WriteAll(this Stream stream, byte[] buffer)
            => stream.WriteAsync(buffer, 0, buffer.Length);
    }
}
