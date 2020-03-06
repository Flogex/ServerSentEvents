using System.Threading.Tasks;
using FluentAssertions;
using ServerSentEvents.Test.Unit.Fakes;
using Xunit;

namespace ServerSentEvents.Test.Unit
{
    public class ServerTests_SendEvent
    {
        private async Task<string> GetResponseBodyAfterEventBeingSent(
            string data,
            string type = null,
            string id = null)
        {
            var sut = new Server();
            var context = FakeHttpContext.GetInstance();
            var client = await HttpClient.NewClient(context);

            await sut.SendEvent(client, data, type, id);

            var body = await context.Response.Body.ReadFromStart();
            return body;
        }

        [Fact]
        public async Task EventDataShouldBeWrittenToBody()
        {
            var body = await GetResponseBodyAfterEventBeingSent("Hello World");
            body.Should().Be("data:Hello World\n\n");
        }

        [Fact]
        public async Task IfEventHasType_TypeShouldBeWrittenToBody()
        {
            var body = await GetResponseBodyAfterEventBeingSent(
                "sampleData",
                type: "sampleType");

            body.Should().Contain("event:sampleType\n");
        }

        [Fact]
        public async Task IfEventTypeEqualsMessage_EventTypeShouldNotBeWritten()
        {
            var body = await GetResponseBodyAfterEventBeingSent(
                "sampleData",
                type: "message");

            body.Should().Be("data:sampleData\n\n");
        }

        [Fact]
        public async Task IfEventHasId_IdShouldBeWrittenToBody()
        {
            var body = await GetResponseBodyAfterEventBeingSent(
                "sampleData",
                id: "sampleId");

            body.Should().Contain("id:sampleId\n");
        }

        [Fact]
        public async Task IfEventHasIdAndType_BothShouldBeWrittenToBody()
        {
            var body = await GetResponseBodyAfterEventBeingSent(
                "sampleData",
                "sampleType",
                "sampleId");

            body.Should().Be("id:sampleId\nevent:sampleType\ndata:sampleData\n\n");
        }

        [Fact]
        public async Task IfEventDataContainsLineFeed_BodyShouldContainMultipleDataLines()
        {
            var body = await GetResponseBodyAfterEventBeingSent("line1\nline2");
            body.Should().Be("data:line1\ndata:line2\n\n");
        }

        [Fact]
        public async Task ConsecutiveAndIrrelevantLineFeedsShouldBeIgnored()
        {
            var body = await GetResponseBodyAfterEventBeingSent("\nline1\n\nline2\n");
            body.Should().Be("data:line1\ndata:line2\n\n");
        }

        [Fact]
        public async Task IfEventDataContaninsCRLF_CarriageReturnShouldBeIgnored()
        {
            var body = await GetResponseBodyAfterEventBeingSent("line1\r\nline2");
            body.Should().Be("data:line1\ndata:line2\n\n");
        }
    }
}
