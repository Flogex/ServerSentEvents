using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using ServerSentEvents.Test.Unit.Fakes;
using Xunit;

namespace ServerSentEvents.Test.Unit
{
    public class ServerTests
    {
        public class WhenAddingClient
        {
            private async Task<HttpResponse> GetHttpResponseAfterAddedAsClient()
            {
                var httpResponse = new FakeHttpResponse();
                var sut = new Server();
                await sut.AddClient(new FakeHttpContext(httpResponse));
                return httpResponse;
            }

            [Fact]
            public async Task HttpResponseStatusShouldBeOK()
            {
                var httpResponse = await GetHttpResponseAfterAddedAsClient();
                httpResponse.StatusCode.Should().Be(200);
            }

            [Fact]
            public async Task HttpResponseContentTypeShouldBeEventStream()
            {
                var httpResponse = await GetHttpResponseAfterAddedAsClient();
                httpResponse.ContentType.Should().Be("text/event-stream");
            }

            [Fact]
            public async Task HttpResponseCacheHeaderShouldIndicateNoCaching()
            {
                var httpResponse = await GetHttpResponseAfterAddedAsClient();
                var cachingHeaders = httpResponse.Headers["Cache-Control"];
                cachingHeaders.Should().HaveCount(1).And.Contain("no-cache");
            }
        }

        public class WhenSendingEventToSpecificClient
        {
            private readonly Server _sut = new Server();

            private async Task<string> GetResponseBodyAfterEventBeingSent(Event @event)
            {
                var httpResponse = new FakeHttpResponse();
                var clientId = await _sut.AddClient(new FakeHttpContext(httpResponse));

                await _sut.SendEvent(clientId, @event);

                var body = await httpResponse.Body.ReadFromStart();
                return body;
            }

            [Fact]
            public void IfClientDoesNotExist_ThrowArgumentException()
            {
                var nonExistentId = ClientId.NewClientId();
                _sut.Invoking(x => x.SendEvent(nonExistentId, default))
                   .Should().Throw<ArgumentException>();
            }

            [Fact]
            public async Task EventDataShouldAppearInHttpResponseBody()
            {
                var @event = new Event("Hello World");
                var body = await GetResponseBodyAfterEventBeingSent(@event);

                body.Should().Be("data:Hello World\n\n");
            }

            [Fact]
            public async Task IfEventHasType_TypeShouldAppearInHttpResponseBody()
            {
                var @event = new Event("sampleData", "sampleType");
                var body = await GetResponseBodyAfterEventBeingSent(@event);

                body.Should().Be("event:sampleType\ndata:sampleData\n\n");
            }

            [Fact]
            public async Task IfEventTypeEqualsMessage_EventTypeShouldNotBeWritten()
            {
                var @event = new Event("sampleData", "message");
                var body = await GetResponseBodyAfterEventBeingSent(@event);

                body.Should().Be("data:sampleData\n\n");
            }

            [Fact]
            public async Task IfEventDataContainsLineFeed_MultipleDataLinesShouldBeWrittenToResponseBody()
            {
                var @event = new Event("line1\nline2");
                var body = await GetResponseBodyAfterEventBeingSent(@event);

                body.Should().Be("data:line1\ndata:line2\n\n");
            }

            [Fact]
            public async Task ConsecutiveAndIrrelevantLineFeedsAreIgnored()
            {
                var @event = new Event("\nline1\n\nline2\n");
                var body = await GetResponseBodyAfterEventBeingSent(@event);

                body.Should().Be("data:line1\ndata:line2\n\n");
            }

            [Fact]
            public async Task IfEventDataContanínsCRLF_CarriageReturnIsIgnored()
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
                var httpResponse = new FakeHttpResponse();
                var clientId = await sut.AddClient(new FakeHttpContext(httpResponse));

                await sut.SendComment(clientId, comment);

                var body = await httpResponse.Body.ReadFromStart();
                return body;
            }

            [Fact]
            public async Task CommentIsPrefixedByColon()
            {
                var body = await GetResponseBodyAfterCommentBeingSent("Hello World");
                body.Should().Be(":Hello World\n\n");
            }

            [Fact]
            public async Task IfCommentStartsWithColon_ColonIsPreservedInResponseBody()
            {
                var body = await GetResponseBodyAfterCommentBeingSent(":comment");
                body.Should().Be("::comment\n\n");
            }

            [Fact]
            public async Task IfCommentHasMultipleLines_MultipleCommentsInResponseBody()
            {
                var body = await GetResponseBodyAfterCommentBeingSent("line1\nline2");
                body.Should().Be(":line1\n:line2\n\n");
            }
        }

        public class WhenConnectionIsAborted
        {
            [Fact]
            public async Task ClientIsRemoved()
            {
                var sut = new Server();
                var context = new FakeHttpContext(new FakeHttpResponse());
                var clientId = await sut.AddClient(context);

                context.Abort();

                sut.Invoking(s => s.SendEvent(clientId, default))
                   .Should().Throw<ArgumentException>()
                            .Where(e => e.ParamName == "id");
            }
        }

        public class WhenRemovingClient
        {
            private readonly Server _sut = new Server();

            [Fact]
            public async Task CannotSendMessageToClient()
            {
                var context = new FakeHttpContext(new FakeHttpResponse());
                var clientId = await _sut.AddClient(context);

                _sut.RemoveClient(clientId);

                _sut.Invoking(s => s.SendEvent(clientId, default))
                   .Should().Throw<ArgumentException>()
                            .Where(e => e.ParamName == "id");
            }

            [Fact]
            public async Task ConnectionToClientIsAborted()
            {
                var context = new FakeHttpContext(new FakeHttpResponse());
                var clientId = await _sut.AddClient(context);

                _sut.RemoveClient(clientId);

                context.RequestAborted.IsCancellationRequested.Should().BeTrue();
            }
        }

        public class WhenSendingWaitRequest
        {
            [Fact]
            public async Task HttpResponseBodyContainsRetryFieldWithReconnectionTime()
            {
                var sut = new Server();
                var httpResponse = new FakeHttpResponse();
                var clientId = await sut.AddClient(new FakeHttpContext(httpResponse));

                await sut.SendWaitRequest(clientId, TimeSpan.FromMilliseconds(1000));

                var body = await httpResponse.Body.ReadFromStart();
                body.Should().Be("retry:1000\n\n");
            }
        }
    }
}
