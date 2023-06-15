#nullable enable
using VRisingServerEvents.Events.EventArgs;
using VRisingServerEvents.Interfaces;
using VRisingServerEvents.Network.Events;
using VRisingServerEvents.Utils;

namespace VRisingServerEvents.Events.Incoming.ActivateVBloodAbility;

/// <summary>
/// Received when a player change a power in the ability bar
/// </summary>
public class ActivateVBloodAbilityEvent : IIncomingEventFactory
{
    public string EventName => "ActivateVBloodAbilityEvent";
    public bool Enabled => true;

    public BaseIncomingEventArgs Build(IncomingNetworkEvent networkEvent)
    {
        var prefabGuid = NetBufferInExtensions.ReadPrefabGUID(networkEvent.NetBufferIn);
        var primarySlot = networkEvent.NetBufferIn.ReadBoolean();

        return new ActivateVBloodAbilityEventArgs(
            prefabGuid,
            primarySlot
        );
    }
}