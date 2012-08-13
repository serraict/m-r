using System;
using EventStore;
using EventStore.Dispatcher;
using Rhino.ServiceBus;
using SimpleCQRS.Events;

namespace SimpleCQRS.ApplicationService
{
    public class EventPublisher : IDispatchCommits
    {
        private readonly IServiceBus _bus;

        public EventPublisher(IServiceBus bus)
        {
            _bus = bus;
        }

        public void Dispose()
        {
            // noop; nothing to dispose for now
        }

        public void Dispatch(Commit commit)
        {
            DispatchCommit(commit, _bus);
        }

        public static void DispatchCommit(Commit commit, IServiceBus bus)
        {
            try
            {
                foreach (var @event in commit.Events)
                {
                    Console.WriteLine("Notifying of {0}", @event.Body);
                    bus.Notify((Event)@event.Body);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}