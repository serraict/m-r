using System;
using System.Collections.Generic;
using System.Reflection;
using EventStore;
using Rhino.ServiceBus;
using Rhino.ServiceBus.Castle;
using Rhino.ServiceBus.Hosting;
using SimpleCQRS.Commands;
using SimpleCQRS.Events;
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

                Container.Install(new BusInstaller());

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

        public static void DispatchCommit(Commit commit)
        {
            try
            {
                foreach (var @event in commit.Events)
                {
                    Console.WriteLine("Notifying of {0}", @event.Body);
                    _bus.Notify((Event) @event.Body);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
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
