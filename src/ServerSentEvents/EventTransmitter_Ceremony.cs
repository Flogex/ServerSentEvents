using System.ComponentModel;

namespace ServerSentEvents
{
    public partial class EventTransmitter
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() => nameof(EventTransmitter);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => base.GetHashCode();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object? obj) => base.Equals(obj);
    }
}
