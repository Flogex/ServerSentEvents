using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using ServerSentEvents.Test.Unit.Fakes;
using Xunit;

namespace ServerSentEvents.Test.Unit
{
    public class HttpClientTests_StopReconnecting
    {
        private async Task<HttpResponse> GetHttpResponseWithReconnectingStopped()
        {
            var context = FakeHttpContext.NewHttpContext();
            await HttpClient.StopReconnecting(context);
            return context.Response;
        }

        [Fact]
        public async Task StatusCodeShouldBeNoContent()
        {
            var response = await GetHttpResponseWithReconnectingStopped();
            response.StatusCode.Should().Be(204,
                because: "a client can be told to stop reconnecting using the" +
                "HTTP 204 No Content response code (HTML Living Standard 9.2.1)");
        }

        [Fact]
        public async Task ContentTypeShouldBeEventStream()
        {
            var response = await GetHttpResponseWithReconnectingStopped();
            response.ContentType.Should().Be("text/event-stream");
        }

        [Fact]
        public async Task ConnectionHeaderShouldBeClose()
        {
            var response = await GetHttpResponseWithReconnectingStopped();
            var connectionHeader = response.Headers["Connection"].Single();
            connectionHeader.Should().Be("close",
                because: "neither the server nor the client should send " +
                "any data after 204 No Content is sent");
        }

        [Fact]
        public async Task ContentLengthShouldBeZero()
        {
            var response = await GetHttpResponseWithReconnectingStopped();
            response.ContentLength.Should().Be(0,
                because: "a server MUST generate a Content-Length field with a value " +
                "of '0' if no payload body is to be sent in the response (RFC 7231)");
        }

        [Fact]
        public async Task ResponseShouldBeCompleted()
        {
            var response = await GetHttpResponseWithReconnectingStopped() as FakeHttpResponse;
            response.HasCompleted.Should().BeTrue(because: "there is no additional content " +
                "to send in the response payload body (RFC 7231)");
        }
    }
}
