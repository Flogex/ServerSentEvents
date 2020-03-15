using System;
using System.Threading.Tasks;
using FluentAssertions;
using ServerSentEvents.Test.Unit.Fakes;
using Xunit;

namespace ServerSentEvents.Test.Unit
{
    public class EventTransmitterTests_BroadcastEvent
    {
        private readonly EventTransmitter _sut = new EventTransmitter();
        private readonly FakeClient[] _clients = new FakeClient[]
        {
            new FakeClient(),
            new FakeClient()
        };

        private async Task AssertAllClientStreamsEqual(string expected)
        {
            foreach (var client in _clients)
            {
                var body = await client.ReadStreamFromStart();
                body.Should().Be(expected);
            }
        }

        [Fact]
        public async Task DataEventShouldBeWrittenToStreamOfAllClients()
        {
            await _sut.BroadcastEvent(
                _clients,
                "sampleData",
                "sampleType",
                "sampleId");

            var expected = "id:sampleId\nevent:sampleType\ndata:sampleData\n\n";
            await AssertAllClientStreamsEqual(expected);
        }

        [Fact]
        public async Task CommentShouldBeWrittenToStreamOfAllClients()
        {
            await _sut.BroadcastComment(_clients, "Hello Worlds");
            await AssertAllClientStreamsEqual(":Hello Worlds\n\n");
        }

        [Fact]
        public async Task WaitRequestShouldBeWrittenToStreamOfAllClients()
        {
            await _sut.BroadcastWaitRequest(
                _clients,
                TimeSpan.FromMilliseconds(1000));

            await AssertAllClientStreamsEqual("retry:1000\n\n");
        }
    }
}
