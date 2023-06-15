#nullable enable
using VRisingServerEvents.Interfaces;

namespace VRisingServerEvents.Events.Internal.UserConnected;

public class UserConnectedEvent : IInternalEventFactory
{
    public string EventName => "UserConnectedEvent";
    public bool Enabled => true;

    public UserConnectedEvent()
    {
        
    }
}