using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Configuration;
using MongoDB.Driver;
// using MongoDB.Driver.Linq; using 1.3.1 - from event store, so annot use linq
using MongoDB.Driver.Builders;
using SimpleCQRS;

namespace CQRSGui.Infra
{
    public class MongoReadModelFacade : IReadModelFacade
    {
        public IEnumerable<InventoryItemListDto> GetInventoryItems()
        {
            return InventoryListView.GetAll();
        }

        public InventoryItemDetailsDto GetInventoryItemDetails(Guid id)
        {
            return InventoryItemDetailView.Get(id);
        }

        public static void RebuildReadModel(int toVersion)
        {
            DropReadModelDatabase();
            RePlayAllEvents(toVersion);
        }

        private static void RePlayAllEvents(int toVersion)
        {
            if (toVersion != int.MaxValue)
                throw new ArgumentOutOfRangeException("toVersion", "partial rebuild not supported yet");

            var store = ServiceLocator.Store;
            var bus = new FakeBus();
            RegisterHandlers.RegisterCommandHandlers(bus, store);
            RegisterHandlers.RegisterEventHandlers(bus);
            var events = store.GetAll();
            foreach (var e in events)
            {
                bus.Publish(e); 
            }
        }

        private static void DropReadModelDatabase()
        {
            var server = GetMongoServer();

            server.DropDatabase("simplecqrs_readmodel");
        }

        public static MongoServer GetMongoServer()
        {
            var connectionStringsSection =
                (ConnectionStringsSection) WebConfigurationManager.GetSection("connectionStrings");
            var conn = connectionStringsSection.ConnectionStrings["readmodel"].ConnectionString;
            var server = MongoServer.Create(conn);
            return server;
        }

        public static MongoDatabase Db()
        {
            return GetMongoServer().GetDatabase("simplecqrs_readmodel");
        }
    }

    public class InventoryItemDetailView : Handles<InventoryItemCreated>, Handles<InventoryItemDeactivated>,
                                           Handles<InventoryItemRenamed>, Handles<ItemsRemovedFromInventory>,
                                           Handles<ItemsCheckedInToInventory>
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

        public void Handle(InventoryItemCreated message)
        {
            coll.Save(new InventoryItemDetailsDto
                {
                    Name = message.Name,
                    Id = message.Id,
                    CurrentCount = 0,
                    Version = message.Version
                });
        }

        public void Handle(InventoryItemDeactivated message)
        {
            coll.Remove(Query.EQ("_id", message.Id));
        }

        public void Handle(InventoryItemRenamed message)
        {
            var item = Get(message.Id);
            item.Name = message.NewName;
            item.Version = message.Version;
            coll.Save(item);
        }

        public void Handle(ItemsRemovedFromInventory message)
        {
            var item = Get(message.Id);
            item.CurrentCount -= message.Count;
            item.Version = message.Version;
            coll.Save(item);
        }

        public void Handle(ItemsCheckedInToInventory message)
        {
            var item = Get(message.Id);
            item.CurrentCount += message.Count;
            item.Version = message.Version;
            coll.Save(item);
        }
    }

    public class InventoryListView : Handles<InventoryItemCreated>, Handles<InventoryItemRenamed>,
                                     Handles<InventoryItemDeactivated>
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

        public void Handle(InventoryItemCreated message)
        {
            coll.Save(new InventoryItemListDto(message.Id, message.Name));
        }

        public void Handle(InventoryItemRenamed message)
        {
            var item = coll.FindOne(Query.EQ("_id", message.Id));
            item.Name = message.NewName;
            coll.Save(item);
        }

        public void Handle(InventoryItemDeactivated message)
        {
            coll.Remove(Query.EQ("_id", message.Id));
        }
    }

}