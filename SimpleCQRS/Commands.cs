using System;
namespace SimpleCQRS.Commands
{
	public class Command : Message
	{
	}
	
	public class DeactivateInventoryItem : Command {
	    public Guid InventoryItemId { get; set; }
        public int OriginalVersion { get; set; }

	    public DeactivateInventoryItem()
	    {
	    }

	    public DeactivateInventoryItem(Guid inventoryItemId, int originalVersion)
	    {
	        InventoryItemId = inventoryItemId;
	        OriginalVersion = originalVersion;
	    }
	}
	
	public class CreateInventoryItem : Command {
        public Guid InventoryItemId { get; set; }
        public string Name { get; set; }

	    public CreateInventoryItem()
	    {
	    }

	    public CreateInventoryItem(Guid inventoryItemId, string name)
        {
			InventoryItemId = inventoryItemId;
			Name = name;
        }
	}
	
	public class RenameInventoryItem : Command {
        public Guid InventoryItemId { get; set; }
        public string NewName { get; set; }
        public int OriginalVersion { get; set; }

	    public RenameInventoryItem()
	    {
	    }

	    public RenameInventoryItem(Guid inventoryItemId, string newName, int originalVersion)
        {
			InventoryItemId = inventoryItemId;
			NewName = newName;
            OriginalVersion = originalVersion;
        }
	}

	public class CheckInItemsToInventory : Command {
        public Guid InventoryItemId { get; set; }
        public int Count { get; set; }
        public int OriginalVersion { get; set; }

	    public CheckInItemsToInventory()
	    {
	    }

	    public CheckInItemsToInventory(Guid inventoryItemId, int count, int originalVersion) {
			InventoryItemId = inventoryItemId;
			Count = count;
		    OriginalVersion = originalVersion;
		}
	}
	
	public class RemoveItemsFromInventory : Command {
        public Guid InventoryItemId { get; set; }
        public int Count { get; set; }
        public int OriginalVersion { get; set; }

	    public RemoveItemsFromInventory()
	    {
	    }

	    public RemoveItemsFromInventory(Guid inventoryItemId, int count, int originalVersion)
        {
			InventoryItemId = inventoryItemId;
			Count = count;
            OriginalVersion = originalVersion;
        }
	}
}
