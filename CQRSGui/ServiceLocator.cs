using SimpleCQRS;

namespace CQRSGui
{
    public static class ServiceLocator
    {
        public static FakeBus Bus { get; set; }
        public static Infra.EventStore Store { get; set; }
    }
}