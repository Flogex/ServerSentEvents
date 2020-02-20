using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ServerSentEvents
{
    internal static class EventSerializer
    {
        private const byte _linefeed = 10;
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
            stream.WriteByte(_linefeed);
        }

        private static async Task WriteEventData(Stream stream, string data)
        {
            await stream.WriteAsync(_dataLabel, 0, _dataLabel.Length);
            var bytes = Encoding.UTF8.GetBytes(data);
            await stream.WriteAsync(bytes, 0, bytes.Length);
            stream.WriteByte(_linefeed);
            stream.WriteByte(_linefeed);
        }
    }
}
