using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Microsoft.AspNetCore.Http;

namespace ServerSentEvents.Test.Performance.UseCases
{
    [MinColumn, MaxColumn, MeanColumn, MedianColumn]
    [MemoryDiagnoser]
    public class SendEvents
    {
        private readonly EventTransmitter _eventTransmitter = new EventTransmitter();
        private IClient _client;

        // Iteration time must be at least 100ms to get valid results
        [Params(100000)]
        public int NumberOfIterations { get; set; }

        [IterationSetup]
        public void Setup()
        {
            HttpContext httpContext = new DefaultHttpContext();
            _client = HttpClient.NewClient(httpContext).GetAwaiter().GetResult();
        }

        [IterationCleanup]
        public void Cleanup() => _client.CloseConnection().GetAwaiter().GetResult();

        [Benchmark]
        public async Task SendEventsToSingleClient()
        {
            for (int i = 0; i < NumberOfIterations * 5; i+=5)
            {
                await _eventTransmitter.SendEvent(
                    _client,
                    "Hello World",
                    "welcomeMessage",
                    i.ToString());

                await _eventTransmitter.SendEvent(
                    _client,
                    "{ \"name\": \"El Escorbuto\", \"author\": \"Sedillo Mateo\" }",
                    "json",
                    (i + 1).ToString());

                await _eventTransmitter.SendEvent(
                    _client,
                    "my.cute.mail@example.com",
                    "email",
                    (i + 2).ToString());

                await _eventTransmitter.SendEvent(
                    _client,
                    "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.",
                    null,
                    (i + 3).ToString());

                await _eventTransmitter.SendEvent(
                    _client,
                    "A text\nthat contains\r\nsome line feeds",
                    null,
                    (i + 4).ToString());
            }
        }

        [Benchmark]
        public async Task SendCommentsToSingleClient()
        {
            for (int i = 0; i < NumberOfIterations; i++)
            {
                await _eventTransmitter.SendComment(
                    _client,
                    "Hello World");

                await _eventTransmitter.SendComment(
                    _client,
                    "{ \"name\": \"El Escorbuto\", \"author\": \"Sedillo Mateo\" }");

                await _eventTransmitter.SendComment(
                    _client,
                    "my.cute.mail@example.com");

                await _eventTransmitter.SendComment(
                    _client,
                    "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.");

                await _eventTransmitter.SendComment(
                    _client,
                    "A text\nthat contains\r\nsome line feeds");
            }
        }

        [Benchmark]
        public async Task SendWaitRequestToSingleClient()
        {
            TimeSpan timespan0 = TimeSpan.FromMilliseconds(1);
            TimeSpan timespan1 = TimeSpan.FromSeconds(1);
            TimeSpan timespan2 = TimeSpan.FromMinutes(1);
            TimeSpan timespan3 = TimeSpan.FromHours(1);
            TimeSpan timespan4 = TimeSpan.FromDays(1);

            for (int i = 0; i < NumberOfIterations; i++)
            {
                await _eventTransmitter.SendWaitRequest(
                    _client,
                    timespan0);

                await _eventTransmitter.SendWaitRequest(
                    _client,
                    timespan1);

                await _eventTransmitter.SendWaitRequest(
                    _client,
                    timespan2);

                await _eventTransmitter.SendWaitRequest(
                    _client,
                    timespan3);

                await _eventTransmitter.SendWaitRequest(
                    _client,
                    timespan4);
            }
        }
    }
}
