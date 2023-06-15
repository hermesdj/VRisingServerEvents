using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using VRisingServerEvents.Events;
using VRisingServerEvents.Network;

namespace VRisingServerEvents;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class EventsPlugin : BasePlugin
{
    internal static ManualLogSource Logger { get; private set; }

    Harmony _harmony;

    public override void Load()
    {
        Logger = Log;
        // Plugin startup logic
        EventPublisher.RegisterEventHandlers();
        EventManager.RegisterEventFactories();
        EventsSerializationHook.Initialize();
        
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} version {MyPluginInfo.PLUGIN_VERSION} is loaded!");

        // Harmony patching
        _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        _harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());
    }

    public override bool Unload()
    {
        EventsSerializationHook.Dispose();
        _harmony?.UnpatchSelf();
        return true;
    }
}