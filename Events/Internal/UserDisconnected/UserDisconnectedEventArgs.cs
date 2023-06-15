#nullable enable
using VRisingServerEvents.Events.EventArgs;

namespace VRisingServerEvents.Events.Internal.UserDisconnected;

public class UserDisconnectedEventArgs : BaseInternalEventArgs
{
    public int ApprovedUserIndex { get; set; }
    public int UserIndex { get; set; }

    public string Reason { get; set; }

    public string ExtraData { get; set; }

    public string CharacterName { get; set; }

    internal UserDisconnectedEventArgs(int approvedUserIndex, int userIndex, string characterName, string reason,
        string extraData)
    {
        ApprovedUserIndex = approvedUserIndex;
        UserIndex = userIndex;
        CharacterName = characterName;
        Reason = reason;
        ExtraData = extraData;
    }
}