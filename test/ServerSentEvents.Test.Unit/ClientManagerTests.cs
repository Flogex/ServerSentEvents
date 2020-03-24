using System;
using FluentAssertions;
using ServerSentEvents.Test.Unit.Fakes;
using Xunit;

namespace ServerSentEvents.Test.Unit
{
    public class ClientManagerTests
    {
        private readonly ClientManager _sut = new EventTransmitter().Clients;

        [Fact]
        public void AddClient_IfClientIsNull_ArgumentNullExceptionShouldBeThrown()
        {
            Action act = () => _sut.Add(null);
            act.Should().Throw<ArgumentNullException>()
               .Which.ParamName.Should().Be("client");
        }

        [Fact]
        public void AddClient_ClientShouldBeAddedToManager()
        {
            var client = new FakeClient();
            _sut.Add(client);
            _sut.GetAll().Should().HaveCount(1).And.Contain(client);
        }

        [Fact]
        public void AddClient_ClientAddedEventShouldBeRaised()
        {
            var client = new FakeClient();
            ClientAddedEventArgs eventArgs = null;
            _sut.ClientAdded += (sender, args) => eventArgs = args;

            _sut.Add(client);

            eventArgs.NewClient.Should().Be(client);
        }

        [Fact]
        public void AddClientRange_AllClientsShouldBeAddedToManager()
        {
            var clients = new FakeClient[]
            {
                new FakeClient(),
                new FakeClient()
            };

            _sut.AddRange(clients);

            _sut.GetAll().Should().Contain(clients);
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
