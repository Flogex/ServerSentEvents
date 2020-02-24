using System.IO;
using System.Threading.Tasks;

namespace ServerSentEvents.Test.Unit
{
    public static class Extensions
    {
        public static async Task<string> ReadFromStart(this Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(stream);
            var content = await reader.ReadToEndAsync();
            return content;
        }
    }
}
