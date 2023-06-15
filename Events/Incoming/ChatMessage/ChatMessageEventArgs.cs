#nullable enable
using ProjectM.Network;
using Unity.Collections;
using VRisingServerEvents.Events.EventArgs;

namespace VRisingServerEvents.Events.Incoming.ChatMessage;

public class ChatMessageEventArgs : BaseIncomingEventArgs
{
    public ChatMessageType MessageType { get; set; }
    public FixedString512 MessageContent { get; set; }
    public NetworkId? WhisperTargetId { get; set; }

    internal ChatMessageEventArgs(ChatMessageType messageType, FixedString512 messageContent, NetworkId? networkId)
    {
        MessageType = messageType;
        MessageContent = messageContent;
        WhisperTargetId = networkId;
    }
}