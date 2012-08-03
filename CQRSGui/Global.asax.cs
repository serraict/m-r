using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CQRSGui.Infra;
using EventStore;
using EventStore.Dispatcher;
using EventStore.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using SimpleCQRS;

namespace CQRSGui
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        private static void DispatchCommit(Commit commit)
        {
            try
            {
                foreach (var @event in commit.Events)
                    ServiceLocator.Bus.Publish((Event)@event.Body);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);

            var bus = new FakeBus();

            var eventStoreWrapper = GetWiredEventStoreWrapper();

            RegisterCommandHandlers(bus, eventStoreWrapper);
            RegisterEventHandlers(bus);
            
            // read model hooked up ... since we have not persisted our read model
            // we'll replay all events to rebuild the read model
            RebuildReadModel(eventStoreWrapper, bus);
            
            ServiceLocator.Bus = bus;
        }

        private static void RegisterEventHandlers(FakeBus bus)
        {
            var detail = new InvenotryItemDetailView();
            bus.RegisterHandler<InventoryItemCreated>(detail.Handle);
            bus.RegisterHandler<InventoryItemDeactivated>(detail.Handle);
            bus.RegisterHandler<InventoryItemRenamed>(detail.Handle);
            bus.RegisterHandler<ItemsCheckedInToInventory>(detail.Handle);
            bus.RegisterHandler<ItemsRemovedFromInventory>(detail.Handle);
            var list = new InventoryListView();
            bus.RegisterHandler<InventoryItemCreated>(list.Handle);
            bus.RegisterHandler<InventoryItemRenamed>(list.Handle);
            bus.RegisterHandler<InventoryItemDeactivated>(list.Handle);
        }

        private static void RegisterCommandHandlers(FakeBus bus, Infra.EventStore eventStoreWrapper)
        {
            var rep = new Repository<InventoryItem>(eventStoreWrapper);
            var commands = new InventoryCommandHandlers(rep);
            bus.RegisterHandler<CheckInItemsToInventory>(commands.Handle);
            bus.RegisterHandler<CreateInventoryItem>(commands.Handle);
            bus.RegisterHandler<DeactivateInventoryItem>(commands.Handle);
            bus.RegisterHandler<RemoveItemsFromInventory>(commands.Handle);
            bus.RegisterHandler<RenameInventoryItem>(commands.Handle);
        }

        private void RebuildReadModel(IGetAllEvents store, FakeBus bus)
        {
            foreach (var e in store.GetAll())
            {
                bus.Publish(e);
            }
        }

        private static CQRSGui.Infra.EventStore GetWiredEventStoreWrapper()
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

            return new CQRSGui.Infra.EventStore(store);
        }
    }
}