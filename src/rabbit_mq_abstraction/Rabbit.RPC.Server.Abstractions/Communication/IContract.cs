namespace Rabbit.RPC.Server.Abstractions.Communication;

/// <summary>
/// Interface defines contracts that will come to your server.
/// Server should declare contracts.
/// Client should declare the same contract that server does.
/// </summary>
public interface IContract
{
    public string OperationName { get; }
}
