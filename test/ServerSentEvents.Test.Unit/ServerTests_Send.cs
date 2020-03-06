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
            var nonExistentId = ClientId.NewClientId();

            sut.Invoking(x => x.Send(nonExistentId, new FakeEvent()))
               .Should().Throw<ArgumentException>();
        }
    }
}
