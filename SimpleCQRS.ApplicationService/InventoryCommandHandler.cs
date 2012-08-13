using System;
using Rhino.ServiceBus;
using SimpleCQRS.Commands;

namespace SimpleCQRS.ApplicationService
{
    public class InventoryCommandHandler : ConsumerOf<Command>
    {
        private readonly IServiceBus _bus;
        private InventoryCommandHandlers _handlers;

        public InventoryCommandHandler(IServiceBus bus, InventoryCommandHandlers handlers)
        {
            _bus = bus;
            _handlers = handlers;
        }

        public void Consume(Command message)
        {
            Console.WriteLine("Received {0}", message);
            _handlers.HandleCommand(message);
            _bus.Reply(message);     // reply to indicate command was handled
            Console.WriteLine("Handled {0}", message);
        }
    }
}