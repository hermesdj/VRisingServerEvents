namespace VRisingServerEvents.Interfaces;

public interface IEventFactory
{
    string EventName { get; }
    bool Enabled { get; }
}