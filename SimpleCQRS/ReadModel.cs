using System;
using System.Collections.Generic;
using SimpleCQRS.Events;

namespace SimpleCQRS
{
    public interface IReadModelFacade
    {
        IEnumerable<InventoryItemListDto> GetInventoryItems();
        InventoryItemDetailsDto GetInventoryItemDetails(Guid id);
    }

    public class InventoryItemDetailsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int CurrentCount { get; set; }
        public int Version { get; set; }

        public InventoryItemDetailsDto()
        {
        }

        public InventoryItemDetailsDto(Guid id, string name, int currentCount, int version)
        {
			Id = id;
			Name = name;
            CurrentCount = currentCount;
            Version = version;
        }
    }

    public class InventoryItemListDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public InventoryItemListDto()
        {
        }

        public InventoryItemListDto(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
