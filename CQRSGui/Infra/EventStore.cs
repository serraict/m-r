﻿using System;
using System.Collections.Generic;
using System.Linq;
using EventStore;
using SimpleCQRS;
using ConcurrencyException = SimpleCQRS.ConcurrencyException;

namespace CQRSGui.Infra
{
    /// <summary>
    /// Utility class to wire SimpleCQRS to use Oliver's event store
    /// </summary>
    public class EventStore : SimpleCQRS.IEventStore
    {
        private IStoreEvents _store;
        private readonly IEventPublisher _publisher;

        public EventStore(IStoreEvents store)
        {
            _store = store;
        }

        public void SaveEvents(Guid aggregateId, IEnumerable<Event> events, int expectedVersion)
        {
            var es = events.ToList();

            using (var stream = _store.OpenStream(aggregateId, 0, int.MaxValue))
            {
                var streamVersion = ToSimpleCQRSRevision(stream);
                if (expectedVersion != -1 && streamVersion != expectedVersion)
                    throw new ConcurrencyException();

                var i = expectedVersion;
                foreach (var @event in es)
                {
                    i++;
                    @event.Version = i;
                    stream.Add(new EventMessage() {Body = @event});
                }

                stream.CommitChanges(Guid.NewGuid());
            }

        }

        private static int ToSimpleCQRSRevision(IEventStream stream)
        {
            return stream.StreamRevision-1;
        }

        public List<Event> GetEventsForAggregate(Guid aggregateId)
        {
            using (var stream = _store.OpenStream(aggregateId,0,int.MaxValue))
            {
                return stream.CommittedEvents.Select(e => e.Body)
                                             .Cast<Event>()
                                             .ToList();
            }
        }
    }
}