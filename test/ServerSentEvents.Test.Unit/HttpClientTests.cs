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
            var context = FakeHttpContext.NewHttpContext();
            var client = await HttpClient.NewClient(context);
            return client.HttpContext.Response;
        }

        [Fact]
        public async Task StatusCodeShouldBeOK()
        {
            var response = await GetHttpResponseOfNewClient();
            response.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task ContentTypeShouldBeEventStream()
        {
            var response = await GetHttpResponseOfNewClient();
            response.ContentType.Should().Be("text/event-stream");
        }

        [Fact]
        public async Task CacheControlHeaderShouldBeNoCache()
        {
            var response = await GetHttpResponseOfNewClient();
            var cachingHeaders = response.Headers["Cache-Control"];
            cachingHeaders.Should().HaveCount(1).And.Contain("no-cache");
        }

        [Fact]
        public async Task IfRequestDoesNotContainLastEventId_LastEventIdPropertyShouldBeNull()
        {
            var context = FakeHttpContext.NewHttpContext();
            var client = await HttpClient.NewClient(context);
            client.LastEventId.Should().BeNull();
        }

        [Fact]
        public async Task IfRequestContainsLastEventId_LastEventIdPropertyShouldBeSetToThisValue()
        {
            var request = new FakeHttpRequest();
            request.Headers.Add("Last-Event-Id", "someId");
            var context = FakeHttpContext.NewHttpContext(request: request);

            var client = await HttpClient.NewClient(context);

            client.LastEventId.Should().Be("someId");
        }
    }
}
