using System;

namespace ServerSentEvents
{
    public readonly struct Event
    {
        public Event(string data, string? type = null)
        {
            Type = type != "message" ? type : null;
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }

        public string Data { get; }

        public string? Type { get; }
    }
}
