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
using SimpleCQRS.Events;

namespace SimpleCQRS.ApplicationService
{
    public class BusInstaller : IWindsorInstaller
    {
        // get rid of these statics later
        private static FakeBus _localbus;
        private EventStore _eventStoreWrapper;

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
            InitBusAndEventStore();

            // when registering instances, Windsor does not do any lifetime management.
            container.Register(
                Component.For<ICommandSender, IEventPublisher>()
                    .Instance(_localbus)
                );
        }

        private void InitBusAndEventStore()
        {
            if (_localbus != null && _eventStoreWrapper != null)
                return;

            _localbus = new FakeBus();
            _eventStoreWrapper = GetWiredEventStoreWrapper();
            RegisterHandlers.RegisterCommandHandlers(_localbus, _eventStoreWrapper);
        }
    
    }
}