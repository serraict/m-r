using System;
using System.IO;
using System.Web.Mvc;
using System.Web.Routing;
using Rhino.ServiceBus;
using Rhino.ServiceBus.Castle;
using Rhino.ServiceBus.Hosting;
using SimpleCQRS.MongoReadModel;
using log4net.Config;

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
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);

            XmlConfigurator.Configure();

            QueueUtil.PrepareQueue("client");

            var host = new DefaultHost();
            host.Start<ClientBootStrapper>();

            ServiceLocator.Bus = (IServiceBus) host.Bus;
            ServiceLocator.ReadModel = new MongoReadModelFacade();
        }
    }

    class ClientBootStrapper : CastleBootStrapper
    {
    }

    public class QueueUtil
    {
        public static void PrepareQueue(string queueName)
        {
            if (string.IsNullOrWhiteSpace(queueName))
            {
                throw new ArgumentNullException("queueName");
            }

            var esentName = queueName + ".esent";
            var subscriptionEsentDb = queueName + "_subscriptions.esent";

            if (Directory.Exists(esentName))
                Directory.Delete(esentName, true);

            if (Directory.Exists(subscriptionEsentDb))
                Directory.Delete(subscriptionEsentDb, true);
        }
    }
}