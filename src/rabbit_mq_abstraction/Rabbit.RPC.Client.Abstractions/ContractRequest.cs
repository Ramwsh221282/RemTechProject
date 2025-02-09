namespace Rabbit.RPC.Client.Abstractions;

internal sealed class ContractRequest<TContract>
    where TContract : IContract
{
    public string Name { get; }
    public TContract Body { get; }

    public ContractRequest(TContract body)
    {
        Body = body;
        Name = typeof(TContract).Name;
    }
}
