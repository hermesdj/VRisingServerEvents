#nullable enable
using VRisingServerEvents.Events.EventArgs;

namespace VRisingServerEvents.Events.Internal.UserConnected;

public class UserConnectedEventArgs : BaseInternalEventArgs
{
    public int ApprovedUserIndex { get; set; }
    public int UserIndex { get; set; }

    public bool IsNewPlayer { get; set; }

    public string CharacterName { get; set; }

    internal UserConnectedEventArgs(int approvedUserIndex, int userIndex, bool isNewPlayer, string characterName)
    {
        ApprovedUserIndex = approvedUserIndex;
        UserIndex = userIndex;
        IsNewPlayer = isNewPlayer;
        CharacterName = characterName;
    }
}