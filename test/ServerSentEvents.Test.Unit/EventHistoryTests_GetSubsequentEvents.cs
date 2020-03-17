using FluentAssertions;
using ServerSentEvents.Events;
using Xunit;

namespace ServerSentEvents.Test.Unit
{
    public class EventHistoryTests_GetSubsequentEvents
    {
        private readonly EventHistory _sut = new EventHistory();

        private void AddEventsInOrder(params Event[] events)
        {
            foreach (var @event in events)
                _sut.Add(@event);
        }

        [Fact]
        public void WhenGettingEventsAfterMostRecent_ShouldReturnEmptyCollection()
        {
            _sut.Add(new Event("_", id: "42"));

            var moreRecentEvents = _sut.GetSubsequentEvents("42");

            moreRecentEvents.Should().BeEmpty();
        }

        [Fact]
        public void WhenGettingEventsAfterSecondMostRecent_ShouldReturnMostRecentEvent()
        {
            var secondMostRecent = new Event("_", id: "1");
            var mostRecent = new Event("_", id: "2");

            AddEventsInOrder(secondMostRecent, mostRecent);

            var moreRecentEvents = _sut.GetSubsequentEvents(secondMostRecent.Id);

            moreRecentEvents.Should().BeEquivalentTo(mostRecent);
        }

        [Fact]
        public void WhenGettingEventsAfterOldest_ShouldReturnAllEventsButTheOldest()
        {
            var first = new Event("_", id: "1");
            var second = new Event("_", id: "2");
            var third = new Event("_", id: null);
            var fourth = new Event("_", id: "3");

            AddEventsInOrder(first, second, third, fourth);

            var moreRecentEvents = _sut.GetSubsequentEvents(first.Id);

            moreRecentEvents.Should().BeEquivalentTo(second, third,
                fourth);
        }

        [Fact]
        public void WhenIdIsNonExistent_ShouldReturnEmptyCollection()
        {
            AddEventsInOrder(
                new Event("_", id: "1"),
                new Event("_", id: "2"));

            var moreRecentEvents = _sut.GetSubsequentEvents("nonExistentId");

            moreRecentEvents.Should().BeEmpty();
        }

        [Fact]
        public void IfOnlyEventsWithoutIdStored_ShouldReturnEmptyCollection()
        {
            AddEventsInOrder(
                new Event("_", id: null),
                new Event("_", id: null));

            var moreRecentEvents = _sut.GetSubsequentEvents("someId");

            moreRecentEvents.Should().BeEmpty();
        }

        [Fact]
        public void ShouldNotReturnEventsEarlierThanEventWithGivenId()
        {
            var first = new Event("_", id: "40");
            var second = new Event("_", id: null);
            var third = new Event("_", id: "42");
            var fourth = new Event("_", id: "43");

            AddEventsInOrder(first, second, third, fourth);

            var moreRecentEvents = _sut.GetSubsequentEvents(third.Id);

            moreRecentEvents.Should().NotContain(new[] { first, second, third });
        }

        [Fact]
        public void IfTwoEventsWithSameIdAreStored_LatestShouldBeUsed()
        {
            var first = new Event("_", id: "1");
            var second = new Event("_", id: "2");
            var third = new Event("_", id: "1");
            var fourth = new Event("_", id: "3");

            AddEventsInOrder(first, second, third, fourth);

            var moreRecentEvents = _sut.GetSubsequentEvents("1");

            moreRecentEvents.Should().NotContain(second)
                            .And.Contain(fourth);
        }

        [Fact]
        public void IfNumberOfStoredEventsExceedsCapacity_EarlierEventsAreRemoved()
        {
            var sut = new EventHistory(1);

            sut.Add(new Event("_", id: "1"));
            sut.Add(new Event("_", id: "2"));

            var moreRecentEvents = sut.GetSubsequentEvents("1");

            moreRecentEvents.Should().BeEmpty("no event with id 1 can be found");
        }

        [Fact]
        public void IfNumberOfStoredEventsExceedsCapacity_OtherEventsCanStillBeRetrieved()
        {
            var sut = new EventHistory(2);

            sut.Add(new Event("_", id: "1"));
            sut.Add(new Event("_", id: "2"));
            sut.Add(new Event("_", id: "3"));

            var moreRecentEvents = sut.GetSubsequentEvents("2");

            moreRecentEvents.Should().ContainSingle(e => e.Id == "3");
        }
    }
}
