using System;

namespace VRisingServerEvents.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class VRisingEventByPatternAttribute : Attribute
{
    public string Pattern { get; set; }

    public VRisingEventByPatternAttribute(string pattern)
    {
        Pattern = pattern;
    }
}