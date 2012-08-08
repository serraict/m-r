using System;
namespace SimpleCQRS.Events
{
	public class Event : Message
	{
        public Guid Id { get; set; }
        public int Version;
	}
	
	public class InventoryItemDeactivated : Event {
	    
        public InventoryItemDeactivated()
	    {
	    }

	    public InventoryItemDeactivated(Guid id)
		{
		    Id = id;
		}
	}

    public class InventoryItemCreated : Event {
		public string Name  { get; set; }

        public InventoryItemCreated()
        {
        }

        public InventoryItemCreated(Guid id, string name) {
			Id = id;
			Name = name;
		}
	}

    public class InventoryItemRenamed : Event
    {
        public string NewName { get; set; }

        public InventoryItemRenamed()
        {
        }

        public InventoryItemRenamed(Guid id, string newName)
        {
            Id = id;
            NewName = newName;
        }
    }

    public class ItemsCheckedInToInventory : Event
    {
        public int Count { get; set; }

        public ItemsCheckedInToInventory()
        {
        }

        public ItemsCheckedInToInventory(Guid id, int count) {
			Id = id;
			Count = count;
		}
	}

    public class ItemsRemovedFromInventory : Event
    {
        public int Count { get; set; }

        public ItemsRemovedFromInventory()
        {
        }

        public ItemsRemovedFromInventory(Guid id, int count) {
			Id = id;
			Count = count;
		}
	}
}

