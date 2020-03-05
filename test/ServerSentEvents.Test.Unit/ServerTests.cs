using System;
using System.Threading.Tasks;
using FluentAssertions;
using ServerSentEvents.Test.Unit.Fakes;
using Xunit;

namespace ServerSentEvents.Test.Unit
{
    public class ServerTests
    {
        public class Send
        {
            [Fact]
            public void IfClientDoesNotExist_ShouldThrowArgumentException()
            {
                var sut = new Server();
                var nonExistentId = ClientId.NewClientId();

                sut.Invoking(x => x.Send(nonExistentId, new FakeEvent()))
                   .Should().Throw<ArgumentException>();
            }
        }

        public class SendEvent
        {
            private async Task<string> GetResponseBodyAfterEventBeingSent(
                string data,
                string type = null,
                string id = null)
            {
                var sut = new Server();
                var context = FakeHttpContext.GetInstance();
                var clientId = await sut.AddClient(context);

                await sut.SendEvent(clientId, data, type, id);

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

        public class SendComment
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

            [Fact]
            public async Task ConsecutiveAndIrrelevantLineFeedsShouldBeIgnored()
            {
                var body = await GetResponseBodyAfterCommentBeingSent("\nline1\n\nline2\n");
                body.Should().Be(":line1\n:line2\n\n");
            }

            [Fact]
            public async Task IfCommentContaninsCRLF_CarriageReturnShouldBeIgnored()
            {
                var body = await GetResponseBodyAfterCommentBeingSent("line1\r\nline2");
                body.Should().Be(":line1\n:line2\n\n");
            }
        }

        public class SendWaitRequest
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
