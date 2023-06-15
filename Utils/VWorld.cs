#nullable enable
using System;
using Unity.Entities;

namespace VRisingServerEvents.Utils;

public static class VWorld
{
    private static World? _serverWorld;

    public static World Server
    {
        get
        {
            if (_serverWorld != null && _serverWorld.IsCreated)
                return _serverWorld;

            _serverWorld = GetWorld("Server")
                           ?? throw new Exception(
                               "There is no Server world (yet). Did you install a server mod on the client?");
            return _serverWorld;
        }
    }

    private static World? GetWorld(string name)
    {
        foreach (var world in World.s_AllWorlds)
        {
            if (world.Name != name)
            {
                continue;
            }

            _serverWorld = world;
            return world;
        }

        return null;
    }
}