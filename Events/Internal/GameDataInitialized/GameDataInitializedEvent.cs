#nullable enable
using VRisingServerEvents.Interfaces;

namespace VRisingServerEvents.Events.Internal.GameDataInitialized;

public class GameDataInitializedEvent : IInternalEventFactory
{
    public string EventName => "GameDataInitializedEvent";
    public bool Enabled => true;
    
}