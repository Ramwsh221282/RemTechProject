using RabbitMQ.Client;

namespace Rabbit.RPC.Client.Abstractions;

/// <summary>
/// Publisher version that creates connection, craetes channel, sends and waits for message then closes channels and connections.
/// </summary>
public sealed class SingleCommunicationPublisher
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

    public async Task<TResponse?> SendCommand<TMessage, TResponse>(
        TMessage message,
        CancellationToken ct = default
    )
    {
        using IConnection connection = await _factory.CreateConnectionAsync(ct);
        using IChannel channel = await connection.CreateChannelAsync(cancellationToken: ct);
        using CommunicationContext<TResponse?> context = new(channel);
        return await context.SendWithValueBack(message, _queueName, ct);
    }
}
