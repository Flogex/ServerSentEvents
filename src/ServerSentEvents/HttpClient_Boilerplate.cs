﻿using System;
using System.Collections.Generic;

namespace ServerSentEvents
{
    public partial class HttpClient
    {
        public override string ToString() => _id.ToString();

        public bool Equals(HttpClient? other) => other is object && _id.Equals(other._id);

        public override bool Equals(object? obj) => obj is HttpClient other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(_id);

        public static bool operator ==(HttpClient a, HttpClient b) => EqualityComparer<HttpClient>.Default.Equals(a, b);

        public static bool operator !=(HttpClient a, HttpClient b) => !(a == b);
    }
}
