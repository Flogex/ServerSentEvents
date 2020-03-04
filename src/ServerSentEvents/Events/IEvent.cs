using System.IO;
using System.Threading.Tasks;

namespace ServerSentEvents.Events
{
    public interface IEvent
    {
        Task WriteToStream(Stream stream);
    }
}
