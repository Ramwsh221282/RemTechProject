using RabbitMQ.Client;

namespace Rabbit.RPC.Client.Abstractions;

/// <summary>
/// Publisher version that creates connection, creates channel, sends and waits for message then closes channels and connections.
/// </summary>
public sealed class SingleCommunicationPublisher : IMessagePublisher
{
    private readonly IConnectionFactory _factory;
    private readonly string _queueName;

    public SingleCommunicationPublisher(
        string queueName,
        string hostname,
        string userName,
        string password
    )
    {
        _queueName = queueName;
        _factory = new ConnectionFactory()
        {
            HostName = hostname,
            UserName = userName,
            Password = password,
        };
    }

    public async Task<ContractActionResult> Send<TMessage>(
        TMessage message,
        CancellationToken ct = default
    )
        where TMessage : IContract
    {
        using IConnection connection = await _factory.CreateConnectionAsync(ct);
        using IChannel channel = await connection.CreateChannelAsync(cancellationToken: ct);
        using CommunicationContext context = new(channel);
        ContractRequest<TMessage> request = new ContractRequest<TMessage>(message);
        ContractActionResult response = await context.Send(request, _queueName, ct);
        return response;
    }

    public void Dispose() { }
}
