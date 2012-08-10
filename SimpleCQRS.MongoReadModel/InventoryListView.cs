using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Rhino.ServiceBus;
using SimpleCQRS.Events;

namespace SimpleCQRS.MongoReadModel
{
    public class InventoryListView : ConsumerOf<InventoryItemCreated>, ConsumerOf<InventoryItemRenamed>,
                                     ConsumerOf<InventoryItemDeactivated>
    {
        private static MongoDatabase db
        {
            get { return MongoReadModelFacade.Db(); }
        }

        private static MongoCollection<InventoryItemListDto> coll
        {
            get { return db.GetCollection<InventoryItemListDto>("InventoryItemListDto"); }
        }

        public static IEnumerable<InventoryItemListDto> GetAll()
        {
            return coll.FindAll().ToList();     // small db, small db
        }

        public void Consume(InventoryItemCreated message)
        {
            coll.Save(new InventoryItemListDto(message.Id, message.Name));
        }

        public void Consume(InventoryItemRenamed message)
        {
            var item = coll.FindOne(Query.EQ("_id", message.Id));
            item.Name = message.NewName;
            coll.Save(item);
        }

        public void Consume(InventoryItemDeactivated message)
        {
            coll.Remove(Query.EQ("_id", message.Id));
        }
    }
}