using System;
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
            Guid id = Guid.NewGuid();

            return new ActionSpecification<Given<InventoryItem>>
                {
                    On = () => new Given<InventoryItem>(
                        new Event[] { new InventoryItemCreated(id, "the name") }),
                    When = given => given.Sut.CheckIn(10),
                    Expect =
                        {
                            given => ((ItemsCheckedInToInventory) given.Sut.GetUncommittedChanges().ElementAt(0)).Count == 10
                        },
                };
        }

        public Specification when_removing_more_items_then_are_available()
        {
            Guid id = Guid.NewGuid();

            return new FailingSpecification<Given<InventoryItem>, InvalidOperationException>
            {
                On = () => new Given<InventoryItem>(new Event[]
                        {
                            new InventoryItemCreated(id, "the name"),
                            new ItemsCheckedInToInventory(id, 2)
                        }),
                When = given => given.Sut.Remove(3),
                Expect =
                        {
                            ex => ex.Message == "only 2 items in stock, cannot remove 3 items"
                        },
            };
        }
    }
}