using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using EventStore;
using EventStore.Dispatcher;
using EventStore.Serialization;
using SimpleCQRS;

namespace CQRSGui
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        private static FakeBus _bus;

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
                    _bus.Publish((Event)@event.Body);
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

            _bus = new FakeBus();

            var eventStoreWrapper = GetWiredEventStoreWrapper();

            var rep = new Repository<InventoryItem>(eventStoreWrapper);
            var commands = new InventoryCommandHandlers(rep);
            _bus.RegisterHandler<CheckInItemsToInventory>(commands.Handle);
            _bus.RegisterHandler<CreateInventoryItem>(commands.Handle);
            _bus.RegisterHandler<DeactivateInventoryItem>(commands.Handle);
            _bus.RegisterHandler<RemoveItemsFromInventory>(commands.Handle);
            _bus.RegisterHandler<RenameInventoryItem>(commands.Handle);
            var detail = new InvenotryItemDetailView();
            _bus.RegisterHandler<InventoryItemCreated>(detail.Handle);
            _bus.RegisterHandler<InventoryItemDeactivated>(detail.Handle);
            _bus.RegisterHandler<InventoryItemRenamed>(detail.Handle);
            _bus.RegisterHandler<ItemsCheckedInToInventory>(detail.Handle);
            _bus.RegisterHandler<ItemsRemovedFromInventory>(detail.Handle);
            var list = new InventoryListView();
            _bus.RegisterHandler<InventoryItemCreated>(list.Handle);
            _bus.RegisterHandler<InventoryItemRenamed>(list.Handle);
            _bus.RegisterHandler<InventoryItemDeactivated>(list.Handle);
            ServiceLocator.Bus = _bus;
        }

        private static IEventStore GetWiredEventStoreWrapper()
        {
            var store = Wireup.Init()
                .UsingMongoPersistence("mongo", new DocumentObjectSerializer())
                .UsingSynchronousDispatchScheduler()
                .DispatchTo(new DelegateMessageDispatcher(DispatchCommit))
                .Build();

            IEventStore eventStoreWrapper = new CQRSGui.Infra.EventStore(store);
            return eventStoreWrapper;
        }
    }
}