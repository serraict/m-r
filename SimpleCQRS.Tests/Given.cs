using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SimpleCQRS.Events;

namespace SimpleCQRS.Tests
{
    public class Given<TEntity>
        where TEntity : AggregateRoot, new()
    {
        public readonly TEntity Sut;
        private readonly List<Event> _events;

        public Given(params Event[] given)
        {
            _events = new List<Event>(given);
            Sut = new TEntity();
            Sut.LoadsFromHistory(given);
        }

        public override string ToString()
        {
            string summary = "Given: " + Sut + "\n";
            summary += " Initially:\n";
            summary = _events.Aggregate(summary, (current, t) => current + ("\t" + t.ToString() + "\n"));
            summary += "\t-\n";
            summary += " Uncommitted events:\n";
            summary = Sut.GetUncommittedChanges().Aggregate(summary, (current, t) => current + ("\t" + t.ToString() + "\n"));
            summary += "\t-\n";
            return summary;
        }
    }

    [TestFixture]
    public class SutToStringTest
    {
        private Given<InventoryItem> _given;

        [SetUp]
        public void SetUp()
        {
            var id = Guid.NewGuid();
            _given = new Given<InventoryItem>(new Event[]
                {
                    new InventoryItemCreated(id, "the name")
                });
            Console.WriteLine(_given);
        }

        [Test]
        public void StringShouldContainNameOfEvents()
        {
            var s = _given.ToString();
            StringAssert.Contains("InventoryItemCreated", s);
        }

        [Test]
        public void StringShouldContainNameOfChangedEvents()
        {
            _given.Sut.CheckIn(10);
            var s = _given.ToString();
            StringAssert.Contains("InventoryItemCreated", s);
            StringAssert.Contains("ItemsCheckedInToInventory", s);
        }
    }
}