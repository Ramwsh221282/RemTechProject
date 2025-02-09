namespace Rabbit.RPC.Server.Abstractions.Communication;

/// <summary>
/// Abstraction for creating entry point to handle broker incoming commands.
/// </summary>
public interface IServerProcess
{
    Task<ContractActionResult> HandleMessage(string receivedJson);
}
