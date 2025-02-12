namespace Rabbit.RPC.Client.Abstractions;

public interface IMessagePublisher
{
    Task<ContractActionResult> Send<TMessage>(TMessage message, CancellationToken ct = default)
        where TMessage : IContract;
}
