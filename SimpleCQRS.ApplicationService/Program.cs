using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using EventStore;
using EventStore.Dispatcher;
using EventStore.Serialization;
using MongoDB.Bson.Serialization;

namespace SimpleCQRS.ApplicationService
{
    class Program
    {
        private static FakeBus _bus; 

        static void Main(string[] args)
        {

            var _bus = new FakeBus();

            var eventStoreWrapper = GetWiredEventStoreWrapper();

            RegisterHandlers.RegisterCommandHandlers(_bus, eventStoreWrapper);
            RegisterHandlers.RegisterEventHandlers(_bus);
        }


        private static void DispatchCommit(Commit commit)
        {
            try
            {
                foreach (var @event in commit.Events)
                    _bus.Publish((Event)@event.Body);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static EventStore GetWiredEventStoreWrapper()
        {
            var types = Assembly.GetAssembly(typeof(SimpleCQRS.Event))
                                        .GetTypes()
                                        .Where(type => type.IsSubclassOf(typeof(SimpleCQRS.Event)));
            foreach (var t in types)
                BsonClassMap.LookupClassMap(t);

            var store = Wireup.Init()
                .UsingMongoPersistence("eventstore", new DocumentObjectSerializer())
                .UsingSynchronousDispatchScheduler()
                .DispatchTo(new DelegateMessageDispatcher(DispatchCommit))
                .Build();

            return new EventStore(store);
        }
    }
}
