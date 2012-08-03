using SimpleCQRS;

namespace CQRSGui.Infra
{
    public class MongoReadModelFacade
    {
         
    }

    public class InventoryItemDetailView : Handles<InventoryItemCreated>, Handles<InventoryItemDeactivated>,
                                           Handles<InventoryItemRenamed>, Handles<ItemsRemovedFromInventory>,
                                           Handles<ItemsCheckedInToInventory>
    {
        public void Handle(InventoryItemCreated message)
        {
            throw new System.NotImplementedException();
        }

        public void Handle(InventoryItemDeactivated message)
        {
            throw new System.NotImplementedException();
        }

        public void Handle(InventoryItemRenamed message)
        {
            throw new System.NotImplementedException();
        }

        public void Handle(ItemsRemovedFromInventory message)
        {
            throw new System.NotImplementedException();
        }

        public void Handle(ItemsCheckedInToInventory message)
        {
            throw new System.NotImplementedException();
        }
    }

    public class InventoryListView : Handles<InventoryItemCreated>, Handles<InventoryItemRenamed>,
                                     Handles<InventoryItemDeactivated>
    {
        public void Handle(InventoryItemCreated message)
        {
            throw new System.NotImplementedException();
        }

        public void Handle(InventoryItemRenamed message)
        {
            throw new System.NotImplementedException();
        }

        public void Handle(InventoryItemDeactivated message)
        {
            throw new System.NotImplementedException();
        }
    }

}