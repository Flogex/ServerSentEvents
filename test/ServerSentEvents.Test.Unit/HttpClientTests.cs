﻿using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using ServerSentEvents.Test.Unit.Fakes;
using Xunit;

namespace ServerSentEvents.Test.Unit
{
    public class HttpClientTests
    {
        [Fact]
        public async Task IfHttpResponseHasAlreadyStarted_ShouldThrowInvalidOperationException()
        {
            var context = FakeHttpContext.NewHttpContext();
            await context.Response.StartAsync();

            Func<Task> createClientAction = async () =>
                await HttpClient.NewClient(context);

            createClientAction.Should().Throw<InvalidOperationException>()
                                       .WithMessage("Response has already started.");
        }

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
            var context = FakeHttpContext.NewHttpContext();
            context.Request.Headers.Add("Last-Event-Id", "someId");

            var client = await HttpClient.NewClient(context);

            client.LastEventId.Should().Be("someId");
        }

        [Fact]
        public async Task WhenConnectionIsClosed_HttpContextShouldBeAborted()
        {
            var context = FakeHttpContext.NewHttpContext();
            var client = await HttpClient.NewClient(context);

            client.CloseConnection();

            context.RequestAborted.IsCancellationRequested.Should().BeTrue();
        }

        [Fact]
        public async Task WhenConnectionIsClosedByServer_ConnectionClosedEventShouldBeRaised()
        {
            var client = await HttpClient.NewClient(FakeHttpContext.NewHttpContext());
            using var eventMonitor = client.Monitor();

            client.CloseConnection();

            eventMonitor.Should().Raise(nameof(client.ConnectionClosed)).WithSender(client);
        }

        [Fact]
        public async Task WhenConnectionIsClosedByClient_ConnectionClosedEventShouldBeRaised()
        {
            var context = FakeHttpContext.NewHttpContext();
            var client = await HttpClient.NewClient(context);
            using var eventMonitor = client.Monitor();

            context.Abort();

            eventMonitor.Should().Raise(nameof(client.ConnectionClosed)).WithSender(client);
        }
    }
}
