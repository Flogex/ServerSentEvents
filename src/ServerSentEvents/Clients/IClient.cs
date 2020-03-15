using System;
using System.IO;
using System.Threading.Tasks;

namespace ServerSentEvents
{
    public interface IClient
    {
        Stream Stream { get; }

        Task CloseConnection();

        event EventHandler? ConnectionClosed;
    }
}
