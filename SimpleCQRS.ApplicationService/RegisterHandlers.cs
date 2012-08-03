namespace SimpleCQRS.ApplicationService
{
    public static class RegisterHandlers
    {

        public static void RegisterEventHandlers(FakeBus bus)
        {
            var detail = new CQRSGui.Infra.InventoryItemDetailView();
            bus.RegisterHandler<InventoryItemCreated>(detail.Handle);
            bus.RegisterHandler<InventoryItemDeactivated>(detail.Handle);
            bus.RegisterHandler<InventoryItemRenamed>(detail.Handle);
            bus.RegisterHandler<ItemsCheckedInToInventory>(detail.Handle);
            bus.RegisterHandler<ItemsRemovedFromInventory>(detail.Handle);
            var list = new CQRSGui.Infra.InventoryListView();
            bus.RegisterHandler<InventoryItemCreated>(list.Handle);
            bus.RegisterHandler<InventoryItemRenamed>(list.Handle);
            bus.RegisterHandler<InventoryItemDeactivated>(list.Handle);
        }

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