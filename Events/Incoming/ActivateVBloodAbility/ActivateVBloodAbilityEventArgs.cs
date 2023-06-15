#nullable enable
using ProjectM;
using VRisingServerEvents.Events.EventArgs;

namespace VRisingServerEvents.Events.Incoming.ActivateVBloodAbility;

public class ActivateVBloodAbilityEventArgs : BaseIncomingEventArgs
{
    public PrefabGUID AbilityGuid { get; }
    public bool PrimarySlot { get; }

    internal ActivateVBloodAbilityEventArgs(PrefabGUID abilityGuid, bool primarySlot)
    {
        AbilityGuid = abilityGuid;
        PrimarySlot = primarySlot;
    }
}