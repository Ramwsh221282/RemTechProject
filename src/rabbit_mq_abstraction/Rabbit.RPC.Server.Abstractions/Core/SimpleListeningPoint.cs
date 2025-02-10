using System.Text;
using System.Text.Json;
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
    public bool IsInitialized { get; private set; }
    public string ServiceName { get; set; } = "Unknown Service Name";

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
        try
        {
            if (IsInitialized)
                return;
            _logger.Information("Starting service: {Name}", ServiceName);
            _connection = await _factory.CreateConnection();
            _channel = await _connection.CreateChannelAsync();
            _consumer = await CreateCommandConsumer();
            uint cleaned = await _channel.QueuePurgeAsync(_queueName);
            _logger.Information("Purged {Amount} of messages from queue", cleaned);
            _consumer.ReceivedAsync += HandleAccepting;
            await StartConsuming(_consumer);
            IsInitialized = true;
            _logger.Information(
                "Service {Name} is initialized and listening to queue: {_queueName}",
                ServiceName,
                _queueName
            );
        }
        catch (Exception ex)
        {
            _logger.Error(
                "Cannot initialize service {ServiceName}. Exception: {Message}",
                ServiceName,
                ex.Message
            );
        }
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
        string receivedJson = Encoding.UTF8.GetString(data);
        _logger.Information(
            "{ServiceName} Received request: {Response}",
            ServiceName,
            receivedJson
        );
        ContractActionResult response = await _process.HandleMessage(receivedJson);
        string responseJson = JsonSerializer.Serialize(response);
        await SendResponse(responseJson, ea);
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
        _logger.Information(
            "{ServiceName} sent response: {responseJson}",
            ServiceName,
            responseJson
        );
    }

    public async ValueTask DisposeAsync()
    {
        await _channel.DisposeAsync();
        await _connection.DisposeAsync();
        _logger.Information("Service {ServiceName} stopped.");
    }
}
