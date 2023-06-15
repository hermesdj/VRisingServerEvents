using ProjectM;
using ProjectM.Network;
using Stunlock.Network;

namespace VRisingServerEvents.Utils;

internal static class NetBufferInExtensions
{
    internal static NetworkId ReadNetworkId(NetBufferIn netBufferIn)
    {
        var index = netBufferIn.ReadRangedInteger(0, 0xffffe);
        var generation = netBufferIn.ReadByte();

        return new NetworkId
        {
            Index = index,
            Generation = generation
        };
    }

    internal static PrefabGUID ReadPrefabGUID(NetBufferIn netBufferIn)
    {
        return new PrefabGUID((int)netBufferIn.ReadUInt32());
    }
}