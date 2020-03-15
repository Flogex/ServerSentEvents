using System;
using System.Threading.Tasks;
using FluentAssertions;
using ServerSentEvents.Test.Unit.Fakes;
using Xunit;

namespace ServerSentEvents.Test.Unit
{
    public class EventTransmitterTests_SendWaitRequest
    {
        [Fact]
        public async Task BodyShouldContainRetryFieldWithReconnectionTime()
        {
            var sut = new EventTransmitter();
            var client = new FakeClient();

            await sut.SendWaitRequest(client, TimeSpan.FromMilliseconds(1000));

            var body = await client.ReadStreamFromStart();
            body.Should().Be("retry:1000\n\n");
        }
    }
}
