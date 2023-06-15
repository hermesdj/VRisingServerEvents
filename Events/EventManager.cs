#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VRisingServerEvents.Events;
using VRisingServerEvents.Interfaces;
using VRisingServerEvents.Network.Events;

namespace VRisingServerEvents.Events;

public static class EventManager
{
    private static readonly Dictionary<string, IIncomingEventFactory> IncomingEventFactories = new();
    private static readonly Dictionary<string, IOutgoingEventFactory> OutgoingEventFactories = new();
    private static readonly Dictionary<string, IInternalEventFactory> InternalEventFactories = new();

    internal static bool HandleIncomingNetworkEvent(IncomingNetworkEvent networkEvent, out bool cancelled)
    {
        cancelled = false;
        var eventName = NetworkEvents.GetNetworkEventName(networkEvent.EventId);

        EventsPlugin.Logger?.LogDebug($"Handling event {eventName}");

        if (!IncomingEventFactories.TryGetValue(eventName, out var eventFactory) || !eventFactory.Enabled)
        {
            EventsPlugin.Logger?.LogDebug(
                $"No Handler found for event {eventName} or EventFactory is Disabled ? {eventFactory?.Enabled ?? false}");
            return false;
        }

        var args = eventFactory.Build(networkEvent);
        args.UserEntity = networkEvent.UserEntity;

        EventsPlugin.Logger?.LogDebug(
            $"Handling Incoming event {eventName} with {eventFactory.GetType().ToString()} and args {args}");
        EventPublisher.HandleIncomingEvent(eventFactory.GetType(), args);

        return true;
    }

    internal static bool HandleOutgoingNetworkEvent(OutgoingNetworkEvent networkEvent, out bool cancelled)
    {
        cancelled = false;
        var eventName = NetworkEvents.GetNetworkEventName(networkEvent.EventId);
        EventsPlugin.Logger?.LogDebug($"Handling outgoing event {eventName}");
        if (!OutgoingEventFactories.TryGetValue(eventName, out var eventFactory) || !eventFactory.Enabled)
        {
            EventsPlugin.Logger?.LogDebug(
                $"No Handler found for event {eventName} or EventFactory is Disabled ? {eventFactory?.Enabled ?? false}");
            return false;
        }

        var args = eventFactory.Build(networkEvent);

        EventsPlugin.Logger?.LogDebug(
            $"Handling Outgoing event {eventName} with {eventFactory.GetType().ToString()} and args {args}");
        EventPublisher.HandleOutgoingEvent(eventFactory.GetType(), args);

        return true;
    }

    internal static void RegisterEventFactories() => RegisterEventFactories(Assembly.GetCallingAssembly());

    private static void RegisterEventFactories(Assembly assembly)
    {
        var incomingType = typeof(IIncomingEventFactory);
        var incomingTypes = assembly.GetTypes()
            .Where(t => incomingType.IsAssignableFrom(t) && t is { IsInterface: false, IsAbstract: false });

        foreach (var incomingEventType in incomingTypes)
        {
            var instance = (IIncomingEventFactory?)Activator.CreateInstance(incomingEventType);
            if (instance != null)
            {
                IncomingEventFactories[instance.EventName] = instance;
            }
        }

        var outgoingType = typeof(IOutgoingEventFactory);
        var outgoingTypes = assembly.GetTypes()
            .Where(t => outgoingType.IsAssignableFrom(t) && t is { IsInterface: false, IsAbstract: false });

        foreach (var outgoingEventType in outgoingTypes)
        {
            var instance = (IOutgoingEventFactory?)Activator.CreateInstance(outgoingEventType);
            if (instance != null)
            {
                OutgoingEventFactories[instance.EventName] = instance;
            }
        }

        var internalType = typeof(IInternalEventFactory);
        var internalTypes = assembly.GetTypes()
            .Where(t => internalType.IsAssignableFrom(t) && t is { IsInterface: false, IsAbstract: false });

        foreach (var internalEventType in internalTypes)
        {
            var instance = (IInternalEventFactory?)Activator.CreateInstance(internalEventType);
            if (instance != null)
            {
                InternalEventFactories[instance.EventName] = instance;
            }
        }
    }

    public static List<string> GetAllEvents()
    {
        var result = new List<string>();
        
        result.AddRange(InternalEventFactories.Keys.Select(internalEventName => $"Internal.{internalEventName}"));
        result.AddRange(IncomingEventFactories.Keys.Select(incomingEventName => $"Incoming.{incomingEventName}"));
        result.AddRange(OutgoingEventFactories.Keys.Select(outgoingEventName => $"Outgoing.{outgoingEventName}"));

        return result;
    }
}