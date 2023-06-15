#nullable enable
using VRisingServerEvents.Events.EventArgs;

namespace VRisingServerEvents.Events.Internal.GameDataInitialized;

public class GameDataInitializedEventArgs : BaseInternalEventArgs
{
    public bool LoadingSuccessful { get; set; }

    public GameDataInitializedEventArgs(bool loadingSuccessfull)
    {
        LoadingSuccessful = loadingSuccessfull;
    }
}