using System;
using System.Collections.Generic;

namespace ServerSentEvents
{
    public partial class Client
    {
        public override string ToString() => _id.ToString();

        public bool Equals(Client? other) => other is object && _id.Equals(other._id);

        public override bool Equals(object? obj) => obj is Client other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(_id);

        public static bool operator ==(Client a, Client b) => EqualityComparer<Client>.Default.Equals(a, b);

        public static bool operator !=(Client a, Client b) => !(a == b);
    }
}
