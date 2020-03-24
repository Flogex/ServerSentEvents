using System;
using System.Threading.Tasks;
using FluentAssertions;
using ServerSentEvents.Events;
using ServerSentEvents.Test.Unit.Fakes;
using Xunit;

namespace ServerSentEvents.Test.Unit
{
    public class EventTransmitterTests_Exceptions
    {
        private readonly EventTransmitter _sut = new EventTransmitter();

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void EnableResend_IfMaxStoredEventsIsNonPositiveNumber_ArgumentExceptionShouldBeThrown(
            int capacity)
        {
            Action act = () => _sut.EnableResend(capacity);
            act.Should().Throw<ArgumentException>()
               .Which.ParamName.Should().Be("maxStoredEvents");
        }

        [Fact]
        public async Task Send_IfClientIsNull_ArgumentNullExceptionShouldBeThrown()
        {
            Func<Task> act = () => _sut.Send(null, new Event("_"));
            var exception = await act.Should().ThrowAsync<ArgumentNullException>();
            exception.Which.ParamName.Should().Be("client");
        }

        [Fact]
        public async Task Send_IfClientStreamIsNull_ArgumentExceptionShouldBeThrown()
        {
            var client = new FakeClient
            {
                Stream = null
            };

            Func<Task> act = () => _sut.Send(client, new Event("_"));
            var exception = await act.Should().ThrowAsync<ArgumentException>();

            exception.WithMessage("*stream*null*")
                     .Which.ParamName.Should().Be("client");
        }

        [Fact]
        public async Task Send_IfEventIsNull_ArgumentNullExceptionShouldBeThrown()
        {
            Func<Task> act = () => _sut.Send(new FakeClient(), null);
            var exception = await act.Should().ThrowAsync<ArgumentNullException>();
            exception.Which.ParamName.Should().Be("event");
        }

        [Fact]
        public async Task SendEvent_IfDataIsNull_ArgumentNullExceptionShouldBeThrown()
        {
            Func<Task> act = () => _sut.SendEvent(new FakeClient(), null);
            var exception = await act.Should().ThrowAsync<ArgumentNullException>();
            exception.Which.ParamName.Should().Be("data");
        }

        [Fact]
        public async Task SendComment_IfCommentIsNull_ArgumentNullExceptionShouldBeThrown()
        {
            Func<Task> act = () => _sut.SendComment(new FakeClient(), null);
            var exception = await act.Should().ThrowAsync<ArgumentNullException>();
            exception.Which.ParamName.Should().Be("comment");
        }

        [Fact]
        public async Task SendWaitRequest_IfReconnectionTimeIsSmallerZero_ArgumentExceptionShouldBeThrown()
        {
            Func<Task> act = () => _sut.SendWaitRequest(new FakeClient(), TimeSpan.FromSeconds(-1));
            var exception = await act.Should().ThrowAsync<ArgumentException>();
            exception.WithMessage("*greater*zero*")
                     .Which.ParamName.Should().Be("reconnectionTime");
        }

        [Fact]
        public async Task Broadcast_IfEventIsNull_ArgumentNullExceptionShouldBeThrown()
        {
            Func<Task> act = () => _sut.Broadcast(null);
            var exception = await act.Should().ThrowAsync<ArgumentNullException>();
            exception.Which.ParamName.Should().Be("event");
        }

        [Fact]
        public async Task BroadcastEvent_IfDataIsNull_ArgumentNullExceptionShouldBeThrown()
        {
            Func<Task> act = () => _sut.BroadcastEvent(null);
            var exception = await act.Should().ThrowAsync<ArgumentNullException>();
            exception.Which.ParamName.Should().Be("data");
        }

        [Fact]
        public async Task BroadcastComment_IfCommentIsNull_ArgumentNullExceptionShouldBeThrown()
        {
            Func<Task> act = () => _sut.BroadcastComment(null);
            var exception = await act.Should().ThrowAsync<ArgumentNullException>();
            exception.Which.ParamName.Should().Be("comment");
        }

        [Fact]
        public async Task BroadcastWaitRequest_IfReconnectionTimeIsSmallerZero_ArgumentExceptionShouldBeThrown()
        {
            Func<Task> act = () => _sut.BroadcastWaitRequest(TimeSpan.FromSeconds(-1));
            var exception = await act.Should().ThrowAsync<ArgumentException>();
            exception.WithMessage("*greater*zero*")
                     .Which.ParamName.Should().Be("reconnectionTime");
        }
    }
}
