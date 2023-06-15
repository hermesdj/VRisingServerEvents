#nullable enable
namespace VRisingServerEvents.Events.EventArgs;

public abstract class BaseEventArgs : System.EventArgs
{
    public bool Cancelled { get; set; } = false;
}