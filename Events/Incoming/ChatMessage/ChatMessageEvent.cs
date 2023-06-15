#nullable enable
using System;
using ProjectM.Network;
using VRisingServerEvents.Events.EventArgs;
using VRisingServerEvents.Interfaces;
using VRisingServerEvents.Network.Events;
using VRisingServerEvents.Utils;

namespace VRisingServerEvents.Events.Incoming.ChatMessage;

/// <summary>
/// Received by the server when a player sends a message on the chat.
/// See ChatMessageType to differentiate the different canals&
/// </summary>
public class ChatMessageEvent : IIncomingEventFactory
{
    public string EventName => "ChatMessageEvent";
    public bool Enabled => true;

    public BaseIncomingEventArgs Build(IncomingNetworkEvent networkEvent)
    {
        var netBufferIn = networkEvent.NetBufferIn;
        var chatMessageType = (ChatMessageType)Enum.ToObject(typeof(ChatMessageType), netBufferIn.ReadByte());
        var messageText = netBufferIn.ReadFixedString512();
        NetworkId? networkId = null;

        if (ChatMessageType.Whisper.Equals(chatMessageType))
        {
            networkId = NetBufferInExtensions.ReadNetworkId(netBufferIn);
        }

        return new ChatMessageEventArgs(chatMessageType, messageText, networkId);
    }
}