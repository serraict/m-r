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


    }
}