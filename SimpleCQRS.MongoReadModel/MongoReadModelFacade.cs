using System;
using System.Collections.Generic;
using MongoDB.Driver;
// using MongoDB.Driver.Linq; using 1.3.1 - from event store, so annot use linq

namespace SimpleCQRS.MongoReadModel
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

        public static MongoServer GetMongoServer()
        {
            //var connectionStringsSection =
            //    (ConnectionStringsSection) WebConfigurationManager.GetSection("connectionStrings");
            //var conn = connectionStringsSection.ConnectionStrings["readmodel"].ConnectionString;
            //var server = MongoServer.Create(conn);
            return MongoServer.Create("mongodb://localhost:27017/simplecqrs_readmodel?safe=true");
        }

        public static MongoDatabase Db()
        {
            return GetMongoServer().GetDatabase("simplecqrs_readmodel");
        }
    }
}