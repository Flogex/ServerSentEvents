﻿namespace ServerSentEvents
{
    public readonly struct Event
    {
        public Event(string data) : this(null, data) { }

        public Event(string? type, string data)
        {
            Type = type != "message" ? type : null;
            Data = data;
        }

        public string? Type { get; }

        public string Data { get; }
    }
}
