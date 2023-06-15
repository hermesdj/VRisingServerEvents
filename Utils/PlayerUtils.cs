using ProjectM.Network;
using Unity.Entities;

namespace VRisingServerEvents.Utils;

public class PlayerUtils
{
    private static EntityManager EntityManager = VWorld.Server.EntityManager;

    public static bool IsNewPlayer(Entity userEntity)
    {
        var userComponent = EntityManager.GetComponentData<User>(userEntity);
        return userComponent.CharacterName.IsEmpty;
    }

    public static string GetCharacterName(Entity userEntity)
    {
        var userComponent = EntityManager.GetComponentData<User>(userEntity);
        return userComponent.CharacterName.ToString();
    }
}