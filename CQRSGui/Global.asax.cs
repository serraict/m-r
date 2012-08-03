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

            RegisterHandlers.RegisterCommandHandlers(bus, eventStoreWrapper);
            RegisterHandlers.RegisterEventHandlers(bus);
                       
            ServiceLocator.Bus = bus;
            ServiceLocator.Store = eventStoreWrapper;
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