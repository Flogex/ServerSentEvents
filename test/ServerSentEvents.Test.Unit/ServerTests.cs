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
    }
}
