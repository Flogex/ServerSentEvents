using System;
using System.Collections.Generic;

namespace ServerSentEvents
{
    public class ClientId : IEquatable<ClientId>
    {
        private readonly Guid _guid;

        private ClientId()
        {
            _guid = Guid.NewGuid();
        }

        public static ClientId NewClientId() => new ClientId();

        public override string ToString() => _guid.ToString();

        public bool Equals(ClientId? other) => other is object && _guid.Equals(other._guid);

        public override bool Equals(object? obj) => obj is ClientId other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(_guid);

        public static bool operator ==(ClientId a, ClientId b) => EqualityComparer<ClientId>.Default.Equals(a, b);

        public static bool operator !=(ClientId a, ClientId b) => !(a == b);
    }
}
