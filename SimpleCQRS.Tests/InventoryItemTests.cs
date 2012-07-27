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

            var spec = new ActionSpecification<InventoryItem>()
            {
                On = () =>
                    {
                        var sut = new InventoryItem();
                        var events = new Event[] {new InventoryItemCreated(id, "the name")};
                        sut.LoadsFromHistory(events);
                        return sut;
                    },
                When = sut => sut.CheckIn(10),
                Expect =
                {
                    sut => ((ItemsCheckedInToInventory)sut.GetUncommittedChanges().ElementAt(0)).Count == 10
                },
            };

            return spec;
        }

    }
}