using System;
using System.Linq;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using EventStore;
using EventStore.Dispatcher;
using EventStore.Serialization;
using MongoDB.Bson.Serialization;
using SimpleCQRS.Commands;
using SimpleCQRS.Events;

namespace SimpleCQRS.ApplicationService
{
    public class BusInstaller : IWindsorInstaller
    {
        private static EventStore GetWiredEventStoreWrapper()
        {
            var types = Assembly.GetAssembly(typeof(Event))
                .GetTypes()
                .Where(type => type.IsSubclassOf(typeof(Event)));
            foreach (var t in types)
                BsonClassMap.LookupClassMap(t);

            var store = Wireup.Init()
                .UsingMongoPersistence("eventstore", new DocumentObjectSerializer())
                .UsingSynchronousDispatchScheduler()
                .DispatchTo(new DelegateMessageDispatcher(Program.DispatchCommit))
                .Build();

            return new EventStore(store);
        }

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IEventStore>().Instance(GetWiredEventStoreWrapper()),
                Component.For(typeof(IRepository<>)).ImplementedBy(typeof(Repository<>)),
                Component.For<InventoryCommandHandlers>().ImplementedBy<InventoryCommandHandlers>()
                );
        }
    }
}