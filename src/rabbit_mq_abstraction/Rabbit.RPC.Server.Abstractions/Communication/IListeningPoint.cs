namespace Rabbit.RPC.Server.Abstractions.Communication;

/// <summary>
/// Rabbit MQ listening point that listens for commands and replies for them.
/// </summary>
public interface IListeningPoint : IAsyncDisposable
{
    Task InitializeListener();
    public bool IsInitialized { get; }
    public string ServiceName { get; set; }
}
