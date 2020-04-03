using System;
using System.Threading.Tasks;
using FluentAssertions;
using ServerSentEvents.Test.Unit.Fakes;
using Xunit;

namespace ServerSentEvents.Test.Unit
{
    public class EventTransmitterTests_Resend
    {
        private readonly EventTransmitter _sut = new EventTransmitter();

        [Fact]
        public async Task WhenAddingClientWithKnownLastEventId_AllSubsequentEventsShouldBeSendToClient()
        {
            _sut.EnableResend(10);

            await _sut.BroadcastEvent("Ignored 1", id: "1");
            await _sut.BroadcastEvent("M2", id: "2");
            await _sut.BroadcastEvent("M3", id: null);
            await _sut.BroadcastEvent("M4", id: "4");

            var client = new FakeClient()
            {
                LastEventId = "1"
            };

            _sut.Clients.Add(client);

            var body = await client.ReadStreamFromStart();
            body.Should().Be(
                "id:2\ndata:M2\n\n" +
                "data:M3\n\n" +
                "id:4\ndata:M4\n\n");
        }

        [Fact]
        public async Task WhenAddingClientAndResendIsDisabled_NothingShouldBeWrittenToStream()
        {
            _sut.DisableResend(); // Just to be sure

            await _sut.BroadcastEvent("M1", id: "1");
            await _sut.BroadcastEvent("M2", id: "2");

            var client = new FakeClient()
            {
                LastEventId = "1"
            };

            _sut.Clients.Add(client);

            var body = await client.ReadStreamFromStart();
            body.Should().BeEmpty();
        }
    }
}
