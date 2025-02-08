using Rabbit.RPC.Server.Abstractions.Communication;
using RabbitMQ.Client;

namespace Rabbit.RPC.Server.Abstractions.Core;

public sealed class SimpleConnectionFactory : ICustomConnectionFactory
{
    private readonly ConnectionFactory _factory;

    public SimpleConnectionFactory(string hostName, string userName, string password)
    {
        _factory = new ConnectionFactory()
        {
            HostName = hostName,
            UserName = userName,
            Password = password,
        };
    }

    public async Task<IConnection> CreateConnection(CancellationToken ct = default) =>
        await _factory.CreateConnectionAsync(ct);
}
