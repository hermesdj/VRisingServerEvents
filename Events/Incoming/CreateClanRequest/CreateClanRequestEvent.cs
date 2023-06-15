#nullable enable
using VRisingServerEvents.Events.EventArgs;
using VRisingServerEvents.Interfaces;
using VRisingServerEvents.Network.Events;

namespace VRisingServerEvents.Events.Incoming.CreateClanRequest;

/// <summary>
/// Received when a player wants to create a clan
/// </summary>
public class CreateClanRequestEvent : IIncomingEventFactory
{
    public string EventName => "CreateClan_Request";
    public bool Enabled => true;

    public BaseIncomingEventArgs Build(IncomingNetworkEvent networkEvent)
    {
        var netBufferIn = networkEvent.NetBufferIn;

        var clanName = netBufferIn.ReadFixedString64();
        var clanMotto = netBufferIn.ReadFixedString64();

        return new CreateClanRequestEventArgs(clanName.ToString(), clanMotto.ToString());
    }
}