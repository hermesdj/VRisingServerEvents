using System;

namespace VRisingServerEvents.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class VRisingEventAttribute : Attribute
{
    public Type SubscribedType { get; }

    public VRisingEventAttribute(Type subscribedType)
    {
        SubscribedType = subscribedType;
    }
}