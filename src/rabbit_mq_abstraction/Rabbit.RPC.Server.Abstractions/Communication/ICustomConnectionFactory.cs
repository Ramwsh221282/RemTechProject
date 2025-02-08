using RabbitMQ.Client;

namespace Rabbit.RPC.Server.Abstractions.Communication;

/// <summary>
/// Abstraction for creating IConnection (rabbitMQ connection) objects.
/// </summary>
public interface ICustomConnectionFactory
{
    Task<IConnection> CreateConnection(CancellationToken ct = default);
}
