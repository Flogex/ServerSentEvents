﻿using System;
using System.Threading.Tasks;
using FluentAssertions;
using ServerSentEvents.Test.Unit.Fakes;
using Xunit;

namespace ServerSentEvents.Test.Unit
{
    public class ServerTests_SendWaitRequest
    {
        [Fact]
        public async Task BodyShouldContainRetryFieldWithReconnectionTime()
        {
            var sut = new Server();
            var context = FakeHttpContext.GetInstance();
            var clientId = await sut.AddClient(context);

            await sut.SendWaitRequest(clientId, TimeSpan.FromMilliseconds(1000));

            var body = await context.Response.Body.ReadFromStart();
            body.Should().Be("retry:1000\n\n");
        }
    }
}