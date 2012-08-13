using System;
using System.Collections.Generic;
using System.Reflection;
using Rhino.ServiceBus;
using Rhino.ServiceBus.Castle;
using Rhino.ServiceBus.Hosting;
using log4net.Config;

namespace SimpleCQRS.ApplicationService
{
    internal class Program
    {
        private static IServiceBus _bus;

        public class BackendBootStrapper : CastleBootStrapper
        {
            protected override void ConfigureContainer()
            {
                base.ConfigureContainer();

                Container.Install(new SimpleCQRSBackendInstaller());

                var asm = Assembly.GetAssembly(typeof (SimpleCQRS.MongoReadModel.InventoryListView));
                RegisterConsumersFrom(asm);
            }
        }

        private static void Main(string[] args)
        {
            XmlConfigurator.Configure();

            QueueUtil.PrepareQueue("backend");

            Console.WriteLine("Starting to listen for incoming messages ... (enter to quit)");

            var host = new DefaultHost();
            host.Start<BackendBootStrapper>();

            _bus = (IServiceBus) host.Bus; 

            Console.ReadLine();
        }
    }
}
