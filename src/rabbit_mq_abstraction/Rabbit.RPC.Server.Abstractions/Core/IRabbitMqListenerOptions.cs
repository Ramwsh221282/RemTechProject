namespace Rabbit.RPC.Server.Abstractions.Core;

public interface IRabbitMqListenerOptions
{
    public string QueueName { get; }
    public string HostName { get; }
    public string UserName { get; }
    public string Password { get; }
}
