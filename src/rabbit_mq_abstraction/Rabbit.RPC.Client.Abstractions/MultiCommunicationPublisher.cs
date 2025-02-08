using RabbitMQ.Client;

namespace Rabbit.RPC.Client.Abstractions;

/// <summary>
/// The publisher variant that sends message untill channel and connection closed. Doesnt' dispose channel and connection per message.
/// </summary>
public sealed class MultiCommunicationPublisher : IDisposable
{
    private readonly IConnectionFactory _factory;
    private readonly string _queueName;

    private IConnection _connection = null!;
    private IChannel _channel = null!;
    private bool _isInitialized;

    public MultiCommunicationPublisher(
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

    private async Task Initialize(CancellationToken ct = default)
    {
        if (_isInitialized)
            return;
        _connection = await _factory.CreateConnectionAsync(ct);
        _channel = await _connection.CreateChannelAsync(cancellationToken: ct);
        _isInitialized = true;
    }

    public async Task<TResponse?> SendCommand<TMessage, TResponse>(
        TMessage message,
        CancellationToken ct = default
    )
    {
        await Initialize(ct);
        using CommunicationContext<TResponse?> context = new(_channel);
        return await context.SendWithValueBack(message, _queueName, ct);
    }

    public void Dispose()
    {
        _channel.Dispose();
        _connection.Dispose();
    }
}
