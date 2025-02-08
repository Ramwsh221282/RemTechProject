namespace Rabbit.RPC.Server.Abstractions.Communication;

/// <summary>
/// Contract handler. Interface should be implemented in concrete handler in server domain.
/// </summary>
/// <typeparam name="TContract">Contract interface that both server and client declare</typeparam>
public interface IContractHandler<in TContract>
    where TContract : class, IContract
{
    Task<string> Handle(TContract contract);
}
