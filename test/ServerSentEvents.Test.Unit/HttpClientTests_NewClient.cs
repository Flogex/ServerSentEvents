using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using ServerSentEvents.Test.Unit.Fakes;
using Xunit;

namespace ServerSentEvents.Test.Unit
{
    public class HttpClientTests_NewClient
    {
        [Fact]
        public async Task IfHttpResponseHasAlreadyStarted_ShouldThrowInvalidOperationException()
        {
            var context = FakeHttpContext.NewHttpContext();
            await context.Response.StartAsync();

            Func<Task> createClientAction = async () =>
                await HttpClient.NewClient(context);

            createClientAction.Should().Throw<ArgumentException>()
                                       .WithMessage("*Response * already started*");
        }

        private async Task<HttpResponse> GetHttpResponseOfNewClient()
        {
            var context = FakeHttpContext.NewHttpContext();
            var sut = await HttpClient.NewClient(context);
            return sut.HttpContext.Response;
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
            var cachingHeaders = response.Headers["Cache-Control"].Single();
            cachingHeaders.Should().Be("no-cache");
        }

        [Fact]
        public async Task ConnectionHeaderShouldBeKeepAlive()
        {
            var response = await GetHttpResponseOfNewClient();
            var cachingHeaders = response.Headers["Connection"].Single();
            cachingHeaders.Should().Be("keep-alive");
        }

        [Fact]
        public async Task ResponseShouldBeStarted()
        {
            var response = await GetHttpResponseOfNewClient();
            response.HasStarted.Should().BeTrue();
        }

        [Fact]
        public async Task IfRequestDoesNotContainLastEventId_LastEventIdPropertyShouldBeNull()
        {
            var context = FakeHttpContext.NewHttpContext();
            var sut = await HttpClient.NewClient(context);
            sut.LastEventId.Should().BeNull();
        }

        [Fact]
        public async Task IfRequestContainsLastEventId_LastEventIdPropertyShouldBeSetToThisValue()
        {
            var context = FakeHttpContext.NewHttpContext();
            context.Request.Headers.Add("Last-Event-Id", "someId");

            var sut = await HttpClient.NewClient(context);

            sut.LastEventId.Should().Be("someId");
        }
    }
}
