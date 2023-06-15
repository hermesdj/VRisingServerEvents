#nullable enable
using VRisingServerEvents.Interfaces;

namespace VRisingServerEvents.Events.Internal.ServerLoadingCompleted;

public class ServerLoadingCompletedEvent : IInternalEventFactory
{
    public string EventName => "ServerLoadingCompletedEvent";
    public bool Enabled => true;
}