using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using ServerSentEvents.Test.Unit.Fakes;
using Xunit;

namespace ServerSentEvents.Test.Unit
{
    public class HttpClientTests_CloseConnection : IAsyncLifetime
    {
        private IClient _sut;
        private HttpContext _context;

        public async Task InitializeAsync()
        {
            _context = FakeHttpContext.NewHttpContext();
            _sut = await HttpClient.NewClient(_context);
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task WhenConnectionIsClosed_HttpContextShouldBeAborted()
        {
            await _sut.CloseConnection();
            _context.RequestAborted.IsCancellationRequested.Should().BeTrue();
        }

        [Fact]
        public async Task WhenConnectionIsClosed_HttpResponseShouldBeCompleted()
        {
            var responseCompleted = false;

            _context.Response.OnCompleted(() =>
            {
                responseCompleted = true;
                return Task.CompletedTask;
            });

            await _sut.CloseConnection();

            responseCompleted.Should().BeTrue();
        }

        [Fact]
        public async Task WhenConnectionIsClosedByServer_ConnectionClosedEventShouldBeRaised()
        {
            using var eventMonitor = _sut.Monitor();

            await _sut.CloseConnection();

            eventMonitor.Should().Raise(nameof(_sut.ConnectionClosed))
                                 .WithSender(_sut);
        }

        [Fact]
        public void WhenConnectionIsClosedByClient_ConnectionClosedEventShouldBeRaised()
        {
            using var eventMonitor = _sut.Monitor();

            _context.Abort();

            eventMonitor.Should().Raise(nameof(_sut.ConnectionClosed))
                                 .WithSender(_sut);
        }
    }
}
