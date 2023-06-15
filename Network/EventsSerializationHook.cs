#nullable enable
using System;
using System.Runtime.InteropServices;
using BepInEx.Unity.IL2CPP.Hook;
using ProjectM.Network;
using Stunlock.Network;
using Unity.Entities;
using VRisingServerEvents.Events;
using VRisingServerEvents.Network.Events;
using VRisingServerEvents.Utils;

namespace VRisingServerEvents.Network;

internal static class EventsSerializationHook
{
    private static INativeDetour? _serializeDetour;
    private static INativeDetour? _deserializeDetour;

    public static SerializeEvent? SerializeOriginal;
    public static DeserializeEvent? DeSerializeOriginal;

    public static void Initialize()
    {
        unsafe
        {
            _serializeDetour = NativeHookUtils.Detour<SerializeEvent>(
                typeof(NetworkEvents_Serialize),
                "SerializeNetworkEvent",
                SerializeHook,
                out SerializeOriginal
            );
            _deserializeDetour = NativeHookUtils.Detour<DeserializeEvent>(
                typeof(NetworkEvents_Serialize),
                "DeserializeNetworkEvent",
                DeserializeHook,
                out DeSerializeOriginal
            );
        }
    }

    public static void Dispose()
    {
        _serializeDetour?.Dispose();
        _deserializeDetour?.Dispose();
    }

    private static unsafe void SerializeHook(IntPtr entityManager, NetworkEventType networkEventType,
        ref NetBufferOut netBufferOut, Entity entity)
    {
        EventsPlugin.Logger?.LogDebug(
            $"Serialize Hook buffer length {netBufferOut.m_bitLength} and event is {NetworkEvents.GetNetworkEventName(networkEventType.EventId)}");
        var em = *(EntityManager*)&entityManager;

        var networkEvent = new OutgoingNetworkEvent(networkEventType, networkEventType.EventId, em, entity);
        if (!EventManager.HandleOutgoingNetworkEvent(networkEvent, out var cancelled))
        {
            EventsPlugin.Logger?.LogWarning(
                $"Unhandled outgoing event: {NetworkEvents.GetNetworkEventName(networkEventType.EventId)}");
        }

        if (cancelled)
        {
            return;
        }

        SerializeOriginal!(entityManager, networkEventType, ref netBufferOut, entity);
    }

    private static void DeserializeHook(IntPtr entityManager, IntPtr commandBuffer,
        ref NetBufferIn netBufferIn, NetworkEvents_Serialize.DeserializeNetworkEventParams eventParams)
    {
        EventsPlugin.Logger?.LogDebug(
            $"Serialize Hook buffer length {netBufferIn.m_bitLength}, readPosition {netBufferIn.m_readPosition}");
        var eventId = netBufferIn.ReadInt32();
        var networkEvent = new IncomingNetworkEvent(netBufferIn, eventId, eventParams);

        if (!EventManager.HandleIncomingNetworkEvent(networkEvent, out var cancelled))
        {
            EventsPlugin.Logger?.LogWarning($"Unhandled incoming event: {NetworkEvents.GetNetworkEventName(eventId)}");
        }

        if (cancelled)
        {
            return;
        }

        netBufferIn.m_readPosition = 72;
        DeSerializeOriginal!(entityManager, commandBuffer, ref netBufferIn, eventParams);
    }

    [UnmanagedFunctionPointer((CallingConvention.StdCall))]
    public delegate void SerializeEvent(
        IntPtr entityManager,
        NetworkEventType networkEventType,
        ref NetBufferOut netBufferOut,
        Entity entity
    );

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void DeserializeEvent(
        IntPtr entityManager,
        IntPtr commandBuffer,
        ref NetBufferIn netBufferIn,
        NetworkEvents_Serialize.DeserializeNetworkEventParams eventParams
    );
}