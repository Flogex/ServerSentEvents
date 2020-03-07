using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ServerSentEvents.Events
{
    public class Event : IEvent
    {
        private static readonly byte[] _idLabel = Encoding.UTF8.GetBytes("id:");
        private static readonly byte[] _eventLabel = Encoding.UTF8.GetBytes("event:");
        private static readonly byte[] _dataLabel = Encoding.UTF8.GetBytes("data:");

        public Event(string data, string? type = null, string? id = null)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
            Type = type != "message" ? type : null;
            Id = id;
        }

        public string Data { get; }

        public string? Type { get; }

        public string? Id { get; }

        public async Task WriteToStream(Stream stream)
        {
            if (Id != null)
                await WriteEventId(stream, Id).ConfigureAwait(false);

            if (Type != null)
                await WriteEventType(stream, Type).ConfigureAwait(false);

            await WriteEventData(stream, Data).ConfigureAwait(false);
            await stream.WriteLineFeed().ConfigureAwait(false);
        }

        private static async Task WriteEventId(Stream stream, string id)
        {
            await stream.WriteAll(_idLabel).ConfigureAwait(false);
            var bytes = Encoding.UTF8.GetBytes(id);
            await stream.WriteAll(bytes).ConfigureAwait(false);
            await stream.WriteLineFeed().ConfigureAwait(false);
        }

        private static async Task WriteEventType(Stream stream, string type)
        {
            await stream.WriteAll(_eventLabel).ConfigureAwait(false);
            var bytes = Encoding.UTF8.GetBytes(type);
            await stream.WriteAll(bytes).ConfigureAwait(false);
            await stream.WriteLineFeed().ConfigureAwait(false);
        }

        private static Task WriteEventData(Stream stream, string data)
            => stream.WriteLabeledLines(_dataLabel, data);
    }
}
