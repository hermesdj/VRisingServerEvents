#nullable enable
using VRisingServerEvents.Events.EventArgs;

namespace VRisingServerEvents.Events.Incoming.CreateClanRequest;

public class CreateClanRequestEventArgs : BaseIncomingEventArgs
{
    public string ClanName { get; }
    public string ClanMotto { get; }

    internal CreateClanRequestEventArgs(string clanName, string clanMotto)
    {
        ClanName = clanName;
        ClanMotto = clanMotto;
    }
}