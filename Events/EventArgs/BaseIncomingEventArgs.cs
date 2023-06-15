#nullable enable
using Unity.Entities;

namespace VRisingServerEvents.Events.EventArgs;

public abstract class BaseIncomingEventArgs : BaseEventArgs
{
    public Entity UserEntity { get; internal set; }
}