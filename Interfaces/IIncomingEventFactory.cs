using VRisingServerEvents.Events.EventArgs;
using VRisingServerEvents.Network.Events;

namespace VRisingServerEvents.Interfaces;

public interface IIncomingEventFactory : IEventFactory
{
    BaseIncomingEventArgs Build(IncomingNetworkEvent networkEvent);
}