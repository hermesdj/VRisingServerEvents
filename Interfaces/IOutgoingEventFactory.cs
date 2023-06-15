using VRisingServerEvents.Events.EventArgs;
using VRisingServerEvents.Network.Events;

namespace VRisingServerEvents.Interfaces;

public interface IOutgoingEventFactory : IEventFactory
{
    BaseEventArgs Build(OutgoingNetworkEvent networkEvent);
}