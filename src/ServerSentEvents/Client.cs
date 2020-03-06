using System;
using System.Collections.Generic;

namespace ServerSentEvents
{
    public class Client : IEquatable<Client>
    {
        private Client()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; }

        public static Client NewClient() => new Client();

        public override string ToString() => Id.ToString();

        public bool Equals(Client? other) => other is object && Id.Equals(other.Id);

        public override bool Equals(object? obj) => obj is Client other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Id);

        public static bool operator ==(Client a, Client b) => EqualityComparer<Client>.Default.Equals(a, b);

        public static bool operator !=(Client a, Client b) => !(a == b);
    }
}
