using Rhino.ServiceBus;
using SimpleCQRS;

namespace CQRSGui
{
    public static class ServiceLocator
    {
        public static IServiceBus Bus { get; set; }
        public static IReadModelFacade ReadModel { get; set; }
    }
}