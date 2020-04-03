using System.ComponentModel;
using System.Diagnostics;

namespace ServerSentEvents
{
    [DebuggerDisplay("ClientManager (Count = {_clients.Count})")]
    public partial class ClientManager
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() => nameof(ClientManager);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => base.GetHashCode();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object? obj) => base.Equals(obj);
    }
}
