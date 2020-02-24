using System;

namespace ServerSentEvents
{
    public readonly struct Event
    {
        public Event(string data, string? type = null, string? id = null)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
            Type = type != "message" ? type : null;
            Id = id;
        }

        public string Data { get; }

        public string? Type { get; }

        public string? Id { get; }
    }
}
