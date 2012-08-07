using System;
using System.Collections.Generic;
using System.Text;
using Rhino.ServiceBus;
using Rhino.ServiceBus.Castle;
using Rhino.ServiceBus.Hosting;
using Rhino.ServiceBus.Impl;
using SimpleCQRS.Commands;
using log4net.Config;

namespace SimpleCQRS.ApplicationService
{
    internal class Program
    {

        public class BackendBootStrapper : CastleBootStrapper
        {
            protected override void ConfigureContainer()
            {
                base.ConfigureContainer();

                Container.Install(new BusInstaller());
            }
        }

        private static void Main(string[] args)
        {
            XmlConfigurator.Configure();

            QueueUtil.PrepareQueue("backend");

            Console.WriteLine("Starting to listen for incoming messages ... (enter to quit)");

            var host = new DefaultHost();
            host.Start<BackendBootStrapper>();

            Console.ReadLine();
        }

    }
    
    public class InventoryCommandHandler : ConsumerOf<Command>
    {
        private readonly IServiceBus _bus;
        private readonly ICommandSender _localbus;

        public InventoryCommandHandler(IServiceBus bus, ICommandSender localbus)
        {
            _bus = bus;
            _localbus = localbus;
        }

        public void Consume(Command message)
        {
            Console.WriteLine("Received {0}", message);
            _localbus.Send(message); // dispatch to internal infrastructure
            _bus.Reply(message);     // reply to indicate command was handled
            Console.WriteLine("Handled {0}", message);
        }
    }
}
