using System.Text;
using Rabbit.RPC.Server.Abstractions.Communication;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;

namespace Rabbit.RPC.Server.Abstractions.Core;

internal sealed class SimpleListeningPoint : IListeningPoint
{
    private readonly ILogger _logger;
    private readonly ICustomConnectionFactory _factory;
    private readonly IServerProcess _process;
    private readonly string _queueName;

    private IConnection _connection = null!;
    private IChannel _channel = null!;
    private AsyncEventingBasicConsumer _consumer = null!;
    private bool _isInitialized;

    public SimpleListeningPoint(
        ICustomConnectionFactory factory,
        IServerProcess process,
        string queueName,
        ILogger logger
    )
    {
        _factory = factory;
        _process = process;
        _queueName = queueName;
        _logger = logger;
    }

    public async Task InitializeListener()
    {
        if (_isInitialized)
            return;
        _connection = await _factory.CreateConnection();
        _channel = await _connection.CreateChannelAsync();
        _consumer = await CreateCommandConsumer();
        _consumer.ReceivedAsync += HandleAccepting;
        await StartConsuming(_consumer);
        _isInitialized = true;
        _logger.Information($"Server is initialized and listening to {_queueName}");
    }

    private async Task<AsyncEventingBasicConsumer> CreateCommandConsumer()
    {
        await _channel.QueueDeclareAsync(
            _queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );
        await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);
        return new AsyncEventingBasicConsumer(_channel);
    }

    private async Task StartConsuming(AsyncEventingBasicConsumer consumer) =>
        await _channel.BasicConsumeAsync(queue: _queueName, autoAck: false, consumer: consumer);

    private async Task HandleAccepting(object model, BasicDeliverEventArgs ea)
    {
        byte[] data = ea.Body.ToArray();
        string json = Encoding.UTF8.GetString(data);
        string response = await _process.HandleMessage(json);
        _logger.Information($"Received request: {response}");
        await SendResponse(response, ea);
    }

    private async Task SendResponse(string responseJson, BasicDeliverEventArgs ea)
    {
        IChannel channel = _consumer.Channel;
        var replyProps = new BasicProperties { CorrelationId = ea.BasicProperties.CorrelationId };
        byte[] responseBytes = Encoding.UTF8.GetBytes(responseJson);

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: ea.BasicProperties.ReplyTo!,
            mandatory: true,
            basicProperties: replyProps,
            body: responseBytes
        );

        await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
        _logger.Information($"Sent response: {responseJson}");
    }

    public async ValueTask DisposeAsync()
    {
        await _channel.DisposeAsync();
        await _connection.DisposeAsync();
    }
}
