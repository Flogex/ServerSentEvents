﻿using System;

namespace ServerSentEvents
{
    public readonly struct Event
    {
        public Event(string data) : this(null, data) { }

        public Event(string? type, string data)
        {
            Type = type != "message" ? type : null;
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }

        public string? Type { get; }

        public string Data { get; }
    }
}