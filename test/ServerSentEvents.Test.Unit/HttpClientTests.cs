using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using ServerSentEvents.Test.Unit.Fakes;
using Xunit;

namespace ServerSentEvents.Test.Unit
{
    public class HttpClientTests
    {
        private async Task<HttpResponse> GetHttpResponseOfNewClient()
        {
            var context = FakeHttpContext.GetInstance();
            var client = await HttpClient.NewClient(context);
            return client.HttpContext.Response;
        }

        [Fact]
        public async Task StatusCodeShouldBeOK()
        {
            var httpResponse = await GetHttpResponseOfNewClient();
            httpResponse.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task ContentTypeShouldBeEventStream()
        {
            var httpResponse = await GetHttpResponseOfNewClient();
            httpResponse.ContentType.Should().Be("text/event-stream");
        }

        [Fact]
        public async Task CacheControlHeaderShouldBeNoCache()
        {
            var httpResponse = await GetHttpResponseOfNewClient();
            var cachingHeaders = httpResponse.Headers["Cache-Control"];
            cachingHeaders.Should().HaveCount(1).And.Contain("no-cache");
        }
    }
}
