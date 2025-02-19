namespace Rabbit.RPC.Client.Abstractions;

public interface IMessagePublisher : IDisposable
{
    Task<ContractActionResult> Send<TMessage>(TMessage message, CancellationToken ct = default)
        where TMessage : IContract;
}
