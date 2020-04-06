using System.Reflection;
using BenchmarkDotNet.Running;

namespace ServerSentEvents.Test.Performance
{
    public class Program
    {
        public static void Main()
            => BenchmarkRunner.Run(Assembly.GetEntryAssembly());
    }
}
