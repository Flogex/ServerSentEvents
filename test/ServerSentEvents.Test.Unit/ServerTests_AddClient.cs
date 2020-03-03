using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using ServerSentEvents.Test.Unit.Fakes;
using Xunit;

namespace ServerSentEvents.Test.Unit
{
    public class ServerTests_AddClient
    {
        private async Task<HttpResponse> GetHttpResponseOfAddedClient()
        {
            var context = FakeHttpContext.GetInstance();
            var sut = new Server();
            await sut.AddClient(context);
            return context.Response;
        }

        [Fact]
        public async Task StatusCodeShouldBeOK()
        {
            var httpResponse = await GetHttpResponseOfAddedClient();
            httpResponse.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task ContentTypeShouldBeEventStream()
        {
            var httpResponse = await GetHttpResponseOfAddedClient();
            httpResponse.ContentType.Should().Be("text/event-stream");
        }

        [Fact]
        public async Task CacheControlHeaderShouldBeNoCache()
        {
            var httpResponse = await GetHttpResponseOfAddedClient();
            var cachingHeaders = httpResponse.Headers["Cache-Control"];
            cachingHeaders.Should().HaveCount(1).And.Contain("no-cache");
        }
    }
}
