using System.IO;

namespace ServerSentEvents
{
    public interface IClient
    {
        Stream Stream { get; }
        void CloseConnection();
    }
}
