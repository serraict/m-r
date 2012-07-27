using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Simple.Testing.ClientFramework;

namespace SimpleCQRS.Tests
{
    [TestFixture]
    public class InventoryItemTests : SpecificationFixture
    {
        public Specification when_checking_in_10_items()
        {
            var id = Guid.NewGuid();

            return new ActionSpecification<InventoryItem>()
            {
                On = () =>
                {
                    var sut = new InventoryItem();
                    var events = new Event[] { new InventoryItemCreated(id, "the name") };
                    sut.LoadsFromHistory(events);
                    return sut;
                },
                When = sut => sut.CheckIn(10),
                Expect =
                {
                    sut => ((ItemsCheckedInToInventory)sut.GetUncommittedChanges().ElementAt(0)).Count == 10
                },
            };
        }

        public Specification when_removing_more_items_then_are_available()
        {
            var id = Guid.NewGuid();

            return new FailingSpecification<InventoryItem, InvalidOperationException>()
            {
                On = () =>
                {
                    var sut = new InventoryItem();
                    var events = new Event[]
                        {
                            new InventoryItemCreated(id, "the name"),
                            new ItemsCheckedInToInventory(id, 1)
                        };
                    sut.LoadsFromHistory(events);
                    return sut;
                },
                When = sut => sut.Remove(2),
                Expect =
                {
                    ex => ex.Message == "only 1 items in stock, cannot remove 2 items"
                },
            };
        }

    }
}