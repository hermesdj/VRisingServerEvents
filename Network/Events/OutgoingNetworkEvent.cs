using ProjectM.Network;
using Unity.Entities;

namespace VRisingServerEvents.Network.Events;

public class OutgoingNetworkEvent
{
    public NetworkEventType NetworkEventType { get; }
    public int EventId { get; }
    public EntityManager EntityManager { get; }
    public Entity Entity { get; }

    internal OutgoingNetworkEvent(NetworkEventType networkEventType, int eventId, EntityManager entityManager,
        Entity entity)
    {
        NetworkEventType = networkEventType;
        EventId = eventId;
        EntityManager = entityManager;
        Entity = entity;
    }
}