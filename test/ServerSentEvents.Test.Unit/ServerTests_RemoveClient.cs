using System;
using System.Threading.Tasks;
using FluentAssertions;
using ServerSentEvents.Test.Unit.Fakes;
using Xunit;

namespace ServerSentEvents.Test.Unit
{
    public class WhenClientIsRemoved
    {
        private readonly Server _sut = new Server();

        [Fact]
        public async Task CannotSendMessageToClient()
        {
            var context = FakeHttpContext.GetInstance();
            var clientId = await _sut.AddClient(context);

            _sut.RemoveClient(clientId);

            _sut.Invoking(s => s.Send(clientId, new FakeEvent()))
               .Should().Throw<ArgumentException>()
                        .Where(e => e.ParamName == "client");
        }

        [Fact]
        public async Task ConnectionToClientIsAborted()
        {
            var context = FakeHttpContext.GetInstance();
            var clientId = await _sut.AddClient(context);

            _sut.RemoveClient(clientId);

            context.RequestAborted.IsCancellationRequested.Should().BeTrue();
        }
    }

    public class WhenConnectionIsAborted
    {
        [Fact]
        public async Task ClientIsRemoved()
        {
            var sut = new Server();
            var context = FakeHttpContext.GetInstance();
            var clientId = await sut.AddClient(context);

            context.Abort();

            // Trying to send event should fail
            sut.Invoking(s => s.Send(clientId, new FakeEvent()))
               .Should().Throw<ArgumentException>()
                        .Where(e => e.ParamName == "client");
        }
    }
}
