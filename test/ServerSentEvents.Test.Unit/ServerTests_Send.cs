using System;
using FluentAssertions;
using ServerSentEvents.Test.Unit.Fakes;
using Xunit;

namespace ServerSentEvents.Test.Unit
{
    public class ServerTests_Send
    {
        [Fact]
        public void IfClientDoesNotExist_ShouldThrowArgumentException()
        {
            var sut = new Server();
            var nonExistentClient = Client.NewClient();

            sut.Invoking(x => x.Send(nonExistentClient, new FakeEvent()))
               .Should().Throw<ArgumentException>();
        }
    }
}
