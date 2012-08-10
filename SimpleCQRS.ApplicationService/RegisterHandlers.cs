using System;
using System.Diagnostics;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Rhino.ServiceBus;
using SimpleCQRS.Commands;
using SimpleCQRS.Events;

namespace SimpleCQRS.ApplicationService
{
    public static class RegisterHandlers
    {
        public static void RegisterCommandHandlers(FakeBus bus, IEventStore eventStoreWrapper)
        {
            var rep = new Repository<InventoryItem>(eventStoreWrapper);
            var commands = new InventoryCommandHandlers(rep);
            bus.RegisterHandler<CheckInItemsToInventory>(commands.Handle);
            bus.RegisterHandler<CreateInventoryItem>(commands.Handle);
            bus.RegisterHandler<DeactivateInventoryItem>(commands.Handle);
            bus.RegisterHandler<RemoveItemsFromInventory>(commands.Handle);
            bus.RegisterHandler<RenameInventoryItem>(commands.Handle);
        }

    }
}