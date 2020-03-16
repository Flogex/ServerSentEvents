using FluentAssertions;
using ServerSentEvents.Test.Unit.Fakes;
using Xunit;

namespace ServerSentEvents.Test.Unit
{
    public class ClientManagerTests
    {
        private readonly ClientManager _sut = new ClientManager();

        [Fact]
        public void AddClient_ClientShouldBeAddedToManager()
        {
            var client = new FakeClient();
            _sut.Add(client);
            _sut.GetAll().Should().HaveCount(1).And.Contain(client);
        }

        [Fact]
        public void RemoveClient_ClientShouldBeRemovedFromManager()
        {
            var client = new FakeClient();
            _sut.Add(client);

            _sut.Remove(client);

            _sut.GetAll().Should().NotContain(client);
        }

        [Fact]
        public void WhenClientConnectionIsClosed_ClientShouldBeRemoved()
        {
            var client = new FakeClient();
            _sut.Add(client);

            client.CloseConnection();

            _sut.GetAll().Should().NotContain(client);
        }
    }
}
