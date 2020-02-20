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

            [Fact]
            public void IfClientDoesNotExist_ThrowArgumentException()
            {
                var nonExistentId = ClientId.NewClientId();
                _sut.Invoking(x => x.SendMessage(nonExistentId, string.Empty))
                   .Should().Throw<ArgumentException>();
            }

            [Fact]
            public async Task EventDataShouldAppearInHttpResponseBody()
            {
                var httpResponse = new FakeHttpResponse();
                var clientId = await _sut.AddClient(httpResponse);
                var @event = new Event("Hello World");

                await _sut.SendMessage(clientId, @event);

                var body = await httpResponse.Body.ReadFromStart();
                body.Should().Be("data: Hello World\n\n");
            }

            [Fact]
            public async Task IfEventHasType_TypeShouldAppearInHttpResponseBody()
            {
                var httpResponse = new FakeHttpResponse();
                var clientId = await _sut.AddClient(httpResponse);
                var @event = new Event("sampleType", "sampleData");
                await _sut.SendMessage(clientId, @event);

                var body = await httpResponse.Body.ReadFromStart();
                body.Should().Be("event: sampleType\ndata: sampleData\n\n");
            }
        }

    }
}
