using FluentAssertions;
using ServerSentEvents.Test.Unit.Fakes;
using Xunit;

namespace ServerSentEvents.Test.Unit
{
    public class EventTransmitterTests_Clients
    {
        private readonly EventTransmitter _sut = new EventTransmitter();

        [Fact]
        public void AddClient_ClientShouldBeAddedToManager()
        {
            var client = new FakeClient();
            _sut.Clients.Add(client);
            _sut.Clients.GetAll().Should().HaveCount(1).And.Contain(client);
        }

        [Fact]
        public void AddClientRange_AllClientsShouldBeAddedToManager()
        {
            var clients = new FakeClient[]
            {
                new FakeClient(),
                new FakeClient()
            };

            _sut.Clients.AddRange(clients);

            _sut.Clients.GetAll().Should().Contain(clients);
        }

        [Fact]
        public void RemoveClient_ClientShouldBeRemovedFromManager()
        {
            var client = new FakeClient();
            _sut.Clients.Add(client);

            _sut.Clients.Remove(client);

            _sut.Clients.GetAll().Should().NotContain(client);
        }

        [Fact]
        public void WhenClientConnectionIsClosed_ClientShouldBeRemoved()
        {
            var client = new FakeClient();
            _sut.Clients.Add(client);

            client.CloseConnection();

            _sut.Clients.GetAll().Should().NotContain(client);
        }
    }
}
