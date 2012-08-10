using System;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Rhino.ServiceBus;
using SimpleCQRS.Events;

namespace SimpleCQRS.MongoReadModel
{
    public class InventoryItemDetailView : ConsumerOf<InventoryItemCreated>, ConsumerOf<InventoryItemDeactivated>,
                                           ConsumerOf<InventoryItemRenamed>, ConsumerOf<ItemsRemovedFromInventory>,
                                           ConsumerOf<ItemsCheckedInToInventory>
    {

        private static MongoDatabase db
        {
            get { return MongoReadModelFacade.Db(); }
        }

        private static MongoCollection<InventoryItemDetailsDto> coll
        {
            get { return db.GetCollection<InventoryItemDetailsDto>("InventoryItemDetailsDto"); }
        }

        public static InventoryItemDetailsDto Get(Guid id)
        {
            return coll.FindOne(Query.EQ("_id", id));
        }

        public void Consume(InventoryItemCreated message)
        {
            coll.Save(new InventoryItemDetailsDto
                {
                    Name = message.Name,
                    Id = message.Id,
                    CurrentCount = 0,
                    Version = message.Version
                });
        }

        public void Consume(InventoryItemDeactivated message)
        {
            coll.Remove(Query.EQ("_id", message.Id));
        }

        public void Consume(InventoryItemRenamed message)
        {
            var item = Get(message.Id);
            item.Name = message.NewName;
            item.Version = message.Version;
            coll.Save(item);
        }

        public void Consume(ItemsRemovedFromInventory message)
        {
            var item = Get(message.Id);
            item.CurrentCount -= message.Count;
            item.Version = message.Version;
            coll.Save(item);
        }

        public void Consume(ItemsCheckedInToInventory message)
        {
            var item = Get(message.Id);
            item.CurrentCount += message.Count;
            item.Version = message.Version;
            coll.Save(item);
        }
    }
}