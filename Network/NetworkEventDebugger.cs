using System.Text.RegularExpressions;

namespace VRisingServerEvents.Network;

public static class NetworkEventDebugger
{
    public static void PrintNetworkEvents()
    {
        var networkEventType = typeof(NetworkEvents);
        var regex = new Regex(@"^EventId_(\w*)$");
        var fields = networkEventType.GetProperties();

        foreach (var member in fields)
        {
            var match = regex.Match(member.Name);
            if (!match.Success) continue;

            var eventId = (int)(member.GetValue(null) ?? -1);
            var networkEventName = NetworkEvents.GetNetworkEventName(eventId);

            EventsPlugin.Logger?.LogInfo($"{member.Name};{eventId};{networkEventName}");
        }
    }
}