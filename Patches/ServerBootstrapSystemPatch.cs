using HarmonyLib;
using ProjectM;
using Stunlock.Network;
using VRisingServerEvents.Events;
using VRisingServerEvents.Events.Internal.GameDataInitialized;
using VRisingServerEvents.Events.Internal.ServerLoadingCompleted;
using VRisingServerEvents.Events.Internal.UserConnected;
using VRisingServerEvents.Events.Internal.UserDisconnected;
using VRisingServerEvents.Utils;

namespace VRisingServerEvents.Patches;

[HarmonyPatch(typeof(ServerBootstrapSystem))]
public class ServerBootstrapSystemPatch
{
    [HarmonyPatch(nameof(ServerBootstrapSystem.OnUserConnected))]
    [HarmonyPrefix]
    public static void OnUserConnectedPreFix(ServerBootstrapSystem __instance, NetConnectionId netConnectionId)
    {
        var approvedUserIndex = __instance._NetEndPointToApprovedUserIndex[netConnectionId];
        var serverClient = __instance._ApprovedUsersLookup[approvedUserIndex];
        var userEntity = serverClient.UserEntity;

        var isNewPlayer = PlayerUtils.IsNewPlayer(userEntity);
        var characterName = PlayerUtils.GetCharacterName(userEntity);

        EventPublisher.HandleInternalEvent(typeof(UserConnectedEvent),
            new UserConnectedEventArgs(approvedUserIndex, userEntity.Index, isNewPlayer, characterName));
    }

    [HarmonyPatch(nameof(ServerBootstrapSystem.OnUserDisconnected))]
    [HarmonyPrefix]
    public static void OnUserDisconnectedPreFix(ServerBootstrapSystem __instance, NetConnectionId netConnectionId,
        ConnectionStatusChangeReason connectionStatusReason,
        string extraData)
    {
        var approvedUserIndex = __instance._NetEndPointToApprovedUserIndex[netConnectionId];
        var serverClient = __instance._ApprovedUsersLookup[approvedUserIndex];
        var userEntity = serverClient.UserEntity;
        var characterName = PlayerUtils.GetCharacterName(userEntity);

        EventPublisher.HandleInternalEvent(typeof(UserDisconnectedEvent),
            new UserDisconnectedEventArgs(approvedUserIndex, userEntity.Index, characterName, connectionStatusReason.ToString(),
                extraData));
    }

    [HarmonyPatch(nameof(ServerBootstrapSystem.OnGameDataInitialized))]
    [HarmonyPostfix]
    public static void OnGameDataInitializedPostFix(ServerBootstrapSystem __instance)
    {
        EventPublisher.HandleInternalEvent(typeof(GameDataInitializedEvent),
            new GameDataInitializedEventArgs(__instance._LoadingSuccessful));
    }

    [HarmonyPatch(nameof(ServerBootstrapSystem.ServerLoadingCompleted))]
    [HarmonyPostfix]
    public static void OnServerLoadingCompleted(ServerBootstrapSystem __instance)
    {
        EventPublisher.HandleInternalEvent(typeof(ServerLoadingCompletedEvent),
            new ServerLoadingCompletedEventArgs(__instance._LoadingSuccessful));
    }
}