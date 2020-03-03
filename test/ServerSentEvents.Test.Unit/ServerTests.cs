using System;
using System.Threading.Tasks;
using FluentAssertions;
using ServerSentEvents.Test.Unit.Fakes;
using Xunit;

namespace ServerSentEvents.Test.Unit
{
    public class ServerTests
    {
        public class WhenSendingEventToSpecificClient
        {
            private readonly Server _sut = new Server();

            private async Task<string> GetResponseBodyAfterEventBeingSent(Event @event)
            {
                var context = FakeHttpContext.GetInstance();
                var clientId = await _sut.AddClient(context);

                await _sut.SendEvent(clientId, @event);

                var body = await context.Response.Body.ReadFromStart();
                return body;
            }

            [Fact]
            public void IfClientDoesNotExist_ShouldThrowArgumentException()
            {
                var nonExistentId = ClientId.NewClientId();
                _sut.Invoking(x => x.SendEvent(nonExistentId, default))
                   .Should().Throw<ArgumentException>();
            }

            [Fact]
            public async Task EventDataShouldBeWrittenToBody()
            {
                var @event = new Event("Hello World");
                var body = await GetResponseBodyAfterEventBeingSent(@event);

                body.Should().Be("data:Hello World\n\n");
            }

            [Fact]
            public async Task IfEventHasType_TypeShouldBeWrittenToBody()
            {
                var @event = new Event("sampleData", type: "sampleType");
                var body = await GetResponseBodyAfterEventBeingSent(@event);

                body.Should().Be("event:sampleType\ndata:sampleData\n\n");
            }

            [Fact]
            public async Task IfEventTypeEqualsMessage_EventTypeShouldNotBeWritten()
            {
                var @event = new Event("sampleData", type: "message");
                var body = await GetResponseBodyAfterEventBeingSent(@event);

                body.Should().Be("data:sampleData\n\n");
            }

            [Fact]
            public async Task IfEventHasId_IdShouldBeWrittenToBody()
            {
                var @event = new Event("sampleData", id: "sampleId");
                var body = await GetResponseBodyAfterEventBeingSent(@event);

                body.Should().Be("id:sampleId\ndata:sampleData\n\n");
            }

            [Fact]
            public async Task IfEventHasIdAndType_BothShouldBeWrittenToBody()
            {
                var @event = new Event("sampleData", "sampleType", "sampleId");
                var body = await GetResponseBodyAfterEventBeingSent(@event);

                body.Should().Be("id:sampleId\nevent:sampleType\ndata:sampleData\n\n");
            }

            [Fact]
            public async Task IfEventDataContainsLineFeed_BodyShouldContainMultipleDataLines()
            {
                var @event = new Event("line1\nline2");
                var body = await GetResponseBodyAfterEventBeingSent(@event);

                body.Should().Be("data:line1\ndata:line2\n\n");
            }

            [Fact]
            public async Task ConsecutiveAndIrrelevantLineFeedsShouldBeIgnored()
            {
                var @event = new Event("\nline1\n\nline2\n");
                var body = await GetResponseBodyAfterEventBeingSent(@event);

                body.Should().Be("data:line1\ndata:line2\n\n");
            }

            [Fact]
            public async Task IfEventDataContaninsCRLF_CarriageReturnShouldBeIgnored()
            {
                var @event = new Event("line1\r\nline2");
                var body = await GetResponseBodyAfterEventBeingSent(@event);

                body.Should().Be("data:line1\ndata:line2\n\n");
            }
        }

        public class WhenSendingCommentToSpecificClient
        {
            private async Task<string> GetResponseBodyAfterCommentBeingSent(string comment)
            {
                var sut = new Server();
                var context = FakeHttpContext.GetInstance();
                var clientId = await sut.AddClient(context);

                await sut.SendComment(clientId, comment);

                var body = await context.Response.Body.ReadFromStart();
                return body;
            }

            [Fact]
            public async Task CommentShouldBePrefixedByColon()
            {
                var body = await GetResponseBodyAfterCommentBeingSent("Hello World");
                body.Should().Be(":Hello World\n\n");
            }

            [Fact]
            public async Task IfCommentStartsWithColon_ColonShouldBePreservedInBody()
            {
                var body = await GetResponseBodyAfterCommentBeingSent(":comment");
                body.Should().Be("::comment\n\n");
            }

            [Fact]
            public async Task IfCommentHasMultipleLines_BodyShouldContainMultipleComments()
            {
                var body = await GetResponseBodyAfterCommentBeingSent("line1\nline2");
                body.Should().Be(":line1\n:line2\n\n");
            }
        }

        public class WhenSendingWaitRequest
        {
            [Fact]
            public async Task BodyShouldContainRetryFieldWithReconnectionTime()
            {
                var sut = new Server();
                var context = FakeHttpContext.GetInstance();
                var clientId = await sut.AddClient(context);

                await sut.SendWaitRequest(clientId, TimeSpan.FromMilliseconds(1000));

                var body = await context.Response.Body.ReadFromStart();
                body.Should().Be("retry:1000\n\n");
            }
        }
    }
}
