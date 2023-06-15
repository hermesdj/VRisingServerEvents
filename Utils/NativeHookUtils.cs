#nullable enable
using System;
using System.Reflection;
using BepInEx.Unity.IL2CPP.Hook;
using HarmonyLib;

namespace VRisingServerEvents.Utils;

public static class NativeHookUtils
{
    public static INativeDetour Detour<T>(Type type, string methodName, T to, out T original) where T : Delegate?
    {
        var method = type.GetMethod(methodName, AccessTools.all);
        return Detour(method, to, out original);
    }

    private static INativeDetour Detour<T>(MethodInfo? method, T to, out T original) where T : Delegate?
    {
        var address = Il2CppMethodResolver.ResolveFromMethodInfo(method);
        EventsPlugin.Logger.LogInfo(
            $"Detouring {method?.DeclaringType?.FullName}.{method?.Name} at {address.ToString("X")}");
        return INativeDetour.CreateAndApply(address, to, out original);
    }
}