using System;

namespace SimpleCQRS.ApplicationService
{

    // todo: re implement this
    public static class RebuildReadModelHelpers
    {
        //public static void RebuildReadModel(int toVersion)
        //{
        //    DropReadModelDatabase();
        //    RePlayAllEvents(toVersion);
        //}

        //private static void RePlayAllEvents(int toVersion)
        //{
        //    if (toVersion != int.MaxValue)
        //        throw new ArgumentOutOfRangeException("toVersion", "partial rebuild not supported yet");

        //    var store = ServiceLocator.Store;
        //    var bus = new FakeBus();
        //    RegisterHandlers.RegisterCommandHandlers(bus, store);
        //    RegisterHandlers.RegisterEventHandlers(bus);
        //    var events = store.GetAll();
        //    foreach (var e in events)
        //    {
        //        bus.Publish(e);
        //    }
        //}

        //private static void DropReadModelDatabase()
        //{
        //    var server = GetMongoServer();

        //    server.DropDatabase("simplecqrs_readmodel");
        //} 
    }
}