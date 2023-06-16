#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using VRisingServerEvents.Attributes;
using VRisingServerEvents.Events.EventArgs;
using VRisingServerEvents.Interfaces;

namespace VRisingServerEvents.Events;

public static class EventPublisher
{
    private static readonly Dictionary<Type, List<EventHandler<BaseIncomingEventArgs>>> IncomingEventHandlers = new();
    private static readonly Dictionary<Type, List<EventHandler<BaseEventArgs>>> OutgoingEventHandlers = new();
    private static readonly Dictionary<Type, List<EventHandler<BaseInternalEventArgs>>> InternalEventHandlers = new();

    private static readonly Dictionary<string, List<EventHandler<BaseEventArgs>>> PatternedEventHandlers = new();

    public static void HandleIncomingEvent(Type eventFactoryType, BaseIncomingEventArgs args)
    {
        var container = (IEventFactory?)Activator.CreateInstance(eventFactoryType);
        if (container == null) return;

        if (!IncomingEventHandlers.TryGetValue(eventFactoryType, out var handlers))
        {
            EventsPlugin.Logger?.LogDebug($"No incoming event handlers found for type {eventFactoryType.ToString()}");
            return;
        }

        EventsPlugin.Logger?.LogDebug($"Found {handlers.Count} Handlers for the event");

        foreach (var handler in handlers)
        {
            handler.Invoke(container, args);
        }

        HandlePatternedEvent(eventFactoryType, args, container);
    }

    public static void HandleOutgoingEvent(Type eventFactoryType, BaseEventArgs args)
    {
        var container = (IEventFactory?)Activator.CreateInstance(eventFactoryType);
        if (container == null) return;

        if (!OutgoingEventHandlers.TryGetValue(eventFactoryType, out var handlers))
        {
            EventsPlugin.Logger?.LogDebug($"No outgoing event handlers found for type {eventFactoryType.ToString()}");
            return;
        }

        foreach (var handler in handlers)
        {
            handler.Invoke(container, args);
        }

        HandlePatternedEvent(eventFactoryType, args, container);
    }

    public static void HandleInternalEvent(Type eventFactoryType, BaseInternalEventArgs args)
    {
        var container = (IEventFactory?)Activator.CreateInstance(eventFactoryType);
        if (container == null) return;

        if (!InternalEventHandlers.TryGetValue(eventFactoryType, out var handlers))
        {
            EventsPlugin.Logger?.LogDebug($"No internal event handlers found for type {eventFactoryType.ToString()}");
            return;
        }

        foreach (var handler in handlers)
        {
            handler.Invoke(container, args);
        }

        HandlePatternedEvent(eventFactoryType, args, container);
    }

    private static void HandlePatternedEvent(Type eventFactoryType, BaseEventArgs args, IEventFactory? container)
    {
        if (container == null) return;

        var matches =
            PatternedEventHandlers.Where((keyValuePair) => Regex.IsMatch(container.EventName, keyValuePair.Key))
                .Select(keyValuePair => keyValuePair.Value)
                .ToList();

        foreach (var handler in matches.SelectMany(handlers => handlers))
        {
            handler.Invoke(container, args);
        }
    }

    public static void RegisterEventHandlers() => RegisterEventHandlers(Assembly.GetCallingAssembly());

    private static void RegisterEventHandlers(Assembly assembly)
    {
        var types = ListAllEventHandlingTypes(assembly);
        EventsPlugin.Logger?.LogInfo($"Found {types.Count} types with VRisingEventHandler attribute");
        foreach (var type in types)
        {
            var container = Activator.CreateInstance(type);
            RegisterTypedEventHandlers(type, container);
            RegisterPatternEventHandlers(type, container);
        }
    }

    private static void RegisterTypedEventHandlers(IReflect type, object? container)
    {
        var methods = ListTypeEventHandlers(type);
        EventsPlugin.Logger?.LogInfo(
            $"Found {methods.Count} in type {type.ToString()} methods with VRisingEvent attribute");
        foreach (var method in methods)
        {
            if (method.GetCustomAttribute(typeof(VRisingEventAttribute), true) is not VRisingEventAttribute attr)
            {
                EventsPlugin.Logger?.LogWarning($"method {method.Name} does not have the VRisingEvent attribute !");
                continue;
            }

            var incomingType = typeof(IIncomingEventFactory);
            var outgoingType = typeof(IOutgoingEventFactory);
            var internalType = typeof(IInternalEventFactory);

            if (incomingType.IsAssignableFrom(attr.SubscribedType))
            {
                RegisterIncomingEventHandler(attr.SubscribedType, container, method);
            }
            else if (outgoingType.IsAssignableFrom(attr.SubscribedType))
            {
                RegisterOutgoingEventHandler(attr.SubscribedType, container, method);
            }
            else if (internalType.IsAssignableFrom(attr.SubscribedType))
            {
                RegisterInternalEventHandler(attr.SubscribedType, container, method);
            }
            else
            {
                EventsPlugin.Logger?.LogWarning(
                    $"attribute subscribed type {attr.SubscribedType.ToString()} is invalid !");
            }
        }
    }

    private static void RegisterPatternEventHandlers(IReflect type, object? container)
    {
        var methods = ListPatternEventHandlers(type);
        foreach (var method in methods)
        {
            if (method.GetCustomAttribute(typeof(VRisingEventByPatternAttribute), true) is not
                VRisingEventByPatternAttribute attr)
            {
                EventsPlugin.Logger?.LogWarning($"method {method.Name} does not have the VRisingEvent attribute !");
                continue;
            }

            var eventHandler = (EventHandler<BaseEventArgs>)Delegate.CreateDelegate(
                typeof(EventHandler<BaseEventArgs>),
                container,
                method
            );

            if (!PatternedEventHandlers.ContainsKey(attr.Pattern))
            {
                PatternedEventHandlers[attr.Pattern] = new List<EventHandler<BaseEventArgs>>();
            }

            PatternedEventHandlers[attr.Pattern].Add(eventHandler);
        }
    }

    private static void RegisterIncomingEventHandler(Type eventFactoryType, object container, MethodInfo method)
    {
        EventsPlugin.Logger?.LogInfo($"Registering incoming event handler for type {eventFactoryType.Name}");
        var eventHandler = (EventHandler<BaseIncomingEventArgs>)Delegate.CreateDelegate(
            typeof(EventHandler<BaseIncomingEventArgs>),
            container,
            method
        );

        if (!IncomingEventHandlers.ContainsKey(eventFactoryType))
        {
            IncomingEventHandlers[eventFactoryType] = new List<EventHandler<BaseIncomingEventArgs>>();
        }

        IncomingEventHandlers[eventFactoryType].Add(eventHandler);
    }

    private static void RegisterOutgoingEventHandler(Type eventFactoryType, object container, MethodInfo method)
    {
        EventsPlugin.Logger?.LogInfo($"Registering outgoing event handler for type {eventFactoryType.ToString()}");
        var eventHandler = (EventHandler<BaseEventArgs>)Delegate.CreateDelegate(
            typeof(EventHandler<BaseEventArgs>),
            container,
            method
        );

        if (!OutgoingEventHandlers.ContainsKey(eventFactoryType))
        {
            OutgoingEventHandlers[eventFactoryType] = new List<EventHandler<BaseEventArgs>>();
        }

        OutgoingEventHandlers[eventFactoryType].Add(eventHandler);
    }

    private static void RegisterInternalEventHandler(Type eventFactoryType, object container, MethodInfo method)
    {
        EventsPlugin.Logger?.LogInfo($"Registering internal event handler for type {eventFactoryType.Name}");
        var eventHandler = (EventHandler<BaseInternalEventArgs>)Delegate.CreateDelegate(
            typeof(EventHandler<BaseInternalEventArgs>),
            container,
            method
        );

        if (!InternalEventHandlers.ContainsKey(eventFactoryType))
        {
            InternalEventHandlers[eventFactoryType] = new List<EventHandler<BaseInternalEventArgs>>();
        }

        InternalEventHandlers[eventFactoryType].Add(eventHandler);
    }

    private static List<MethodInfo> ListTypeEventHandlers(IReflect type)
    {
        return type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(method => method.IsDefined(typeof(VRisingEventAttribute))).ToList();
    }

    private static List<MethodInfo> ListPatternEventHandlers(IReflect type)
    {
        return type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(method => method.IsDefined(typeof(VRisingEventByPatternAttribute))).ToList();
    }

    private static List<Type> ListAllEventHandlingTypes(Assembly assembly)
    {
        return assembly.GetTypes().Where(type => type.IsDefined(typeof(VRisingEventHandlerAttribute))).ToList();
    }
}