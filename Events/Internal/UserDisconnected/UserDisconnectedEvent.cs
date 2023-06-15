#nullable enable
using VRisingServerEvents.Interfaces;

namespace VRisingServerEvents.Events.Internal.UserDisconnected;

public class UserDisconnectedEvent : IInternalEventFactory
{
    public string EventName => "UserDisconnectedEvent";
    public bool Enabled => true;
}