using System;
namespace SimpleCQRS
{
	public class Event : Message
	{
        public int Version;
	}
	
	public class InventoryItemDeactivated : Event {
        public Guid Id { get; set; }

	    public InventoryItemDeactivated(Guid id)
		{
		    Id = id;
		}
	}

    public class InventoryItemCreated : Event {
        public Guid Id { get; set; }
		public string Name  { get; set; }
		public InventoryItemCreated(Guid id, string name) {
			Id = id;
			Name = name;
		}
	}

    public class InventoryItemRenamed : Event
    {
        public Guid Id { get; set; }
        public string NewName { get; set; }
        public InventoryItemRenamed(Guid id, string newName)
        {
            Id = id;
            NewName = newName;
        }
    }

    public class ItemsCheckedInToInventory : Event
    {
        public Guid Id { get; set; }
        public int Count { get; set; }
 
        public ItemsCheckedInToInventory(Guid id, int count) {
			Id = id;
			Count = count;
		}
	}

    public class ItemsRemovedFromInventory : Event
    {
        public Guid Id { get; set; }
        public int Count { get; set; }
 
        public ItemsRemovedFromInventory(Guid id, int count) {
			Id = id;
			Count = count;
		}
	}
}

