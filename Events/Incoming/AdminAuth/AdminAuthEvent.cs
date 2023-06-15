#nullable enable
using VRisingServerEvents.Events.EventArgs;
using VRisingServerEvents.Interfaces;
using VRisingServerEvents.Network.Events;

namespace VRisingServerEvents.Events.Incoming.AdminAuth;

/// <summary>
/// Received when a player wants elevated privileges
/// </summary>
public class AdminAuthEvent : IIncomingEventFactory
{
    public string EventName => "AdminAuthEvent";
    public bool Enabled => true;

    public BaseIncomingEventArgs Build(IncomingNetworkEvent networkEvent)
    {
        return new AdminAuthEventArgs();
    }
}