using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
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
                await sut.AddClient(httpResponse);
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
                var clientId = await _sut.AddClient(httpResponse);

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

                body.Should().Be("data: Hello World\n\n");
            }

            [Fact]
            public async Task IfEventHasType_TypeShouldAppearInHttpResponseBody()
            {
                var @event = new Event("sampleType", "sampleData");
                var body = await GetResponseBodyAfterEventBeingSent(@event);

                body.Should().Be("event: sampleType\ndata: sampleData\n\n");
            }

            [Fact]
            public async Task IfEventTypeEqualsMessage_EventTypeShouldNotBeWritten()
            {
                var @event = new Event("message", "sampleData");
                var body = await GetResponseBodyAfterEventBeingSent(@event);

                body.Should().Be("data: sampleData\n\n");
            }
        }

    }
}
