#nullable enable
using VRisingServerEvents.Events.EventArgs;

namespace VRisingServerEvents.Events.Internal.ServerLoadingCompleted;

public class ServerLoadingCompletedEventArgs : BaseInternalEventArgs
{
    public bool LoadingSuccessful { get; set; }

    public ServerLoadingCompletedEventArgs(bool loadingSuccessful)
    {
        LoadingSuccessful = loadingSuccessful;
    }
}