using System;
using System.Collections.Generic;
using System.Linq;
using ServerSentEvents.Events;

namespace ServerSentEvents
{
    internal class EventHistory
    {
        private readonly int _capacity;
        // Most recent events at start of list
        private readonly LinkedList<Event> _events = new LinkedList<Event>();

        public EventHistory(int capacity = 50)
        {
            if (capacity <= 0)
            {
                throw new ArgumentException(
                    "Capacity must be greater zero",
                    nameof(capacity));
            }

            _capacity = capacity;
        }

        public void Add(Event @event)
        {
            if (_events.Count == _capacity)
                _events.RemoveLast();

            _events.AddFirst(@event);
        }

        public IEnumerable<Event> GetSubsequentEvents(string eventId)
        {
            var eventIndex = _events.Find(e => e.Id == eventId);
            // Takes zero elements if eventIndex == -1
            return _events.Take(eventIndex).Reverse();
        }
    }

    internal static class LinkedListExtensions
    {
        public static int Find<T>(
            this LinkedList<T> list,
            Predicate<T> predicate)
        {
            var index = 0;
            foreach (var item in list)
            {
                if (predicate(item))
                    return index;

                index++;
            }

            return -1;
        }
    }
}
