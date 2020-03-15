using System.Threading.Tasks;
using FluentAssertions;
using ServerSentEvents.Test.Unit.Fakes;
using Xunit;

namespace ServerSentEvents.Test.Unit
{
    public class HttpClientTests_CloseConnection
    {
        [Fact]
        public async Task WhenConnectionIsClosed_HttpContextShouldBeAborted()
        {
            var context = FakeHttpContext.NewHttpContext();
            var sut = await HttpClient.NewClient(context);

            await sut.CloseConnection();

            context.RequestAborted.IsCancellationRequested.Should().BeTrue();
        }

        [Fact]
        public async Task WhenConnectionIsClosed_HttpResponseShouldBeCompleted()
        {
            var context = FakeHttpContext.NewHttpContext();
            var sut = await HttpClient.NewClient(context);
            var responseCompleted = false;

            context.Response.OnCompleted(() =>
            {
                responseCompleted = true;
                return Task.CompletedTask;
            });

            await sut.CloseConnection();

            responseCompleted.Should().BeTrue();
        }

        [Fact]
        public async Task WhenConnectionIsClosedByServer_ConnectionClosedEventShouldBeRaised()
        {
            var sut = await HttpClient.NewClient(FakeHttpContext.NewHttpContext());
            using var eventMonitor = sut.Monitor();

            await sut.CloseConnection();

            eventMonitor.Should().Raise(nameof(sut.ConnectionClosed)).WithSender(sut);
        }

        [Fact]
        public async Task WhenConnectionIsClosedByClient_ConnectionClosedEventShouldBeRaised()
        {
            var context = FakeHttpContext.NewHttpContext();
            var sut = await HttpClient.NewClient(context);
            using var eventMonitor = sut.Monitor();

            context.Abort();

            eventMonitor.Should().Raise(nameof(sut.ConnectionClosed)).WithSender(sut);
        }
    }
}
