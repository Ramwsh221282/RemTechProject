using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RemTechCommon.Utils.Converters;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace Rabbit.RPC.Client.Abstractions;

internal sealed class CommunicationContext<TResponse> : IDisposable
{
    private readonly ConcurrentDictionary<string, TaskCompletionSource<TResponse?>> _callbacks = [];
    private readonly IChannel _channel;
    private readonly AsyncEventingBasicConsumer _consumer;
    private readonly ILogger _logger = new LoggerConfiguration()
        .WriteTo.Console()
        .WriteTo.Debug()
        .CreateLogger();

    public CommunicationContext(IChannel channel)
    {
        _channel = channel;
        _consumer = new AsyncEventingBasicConsumer(_channel);
        _consumer.ReceivedAsync += CreateCallbackListener;
    }

    public async Task<TResponse?> SendWithValueBack<TMessage>(
        TMessage message,
        string queueName,
        CancellationToken ct = default
    )
    {
        string replyQueueName = await CreateTemporaryQueueName(ct);
        BasicProperties properties = await GenerateCorellationProperties(
            replyQueueName,
            message,
            ct
        );
        TaskCompletionSource<TResponse?> callback = RegisterReplyQueue(properties);
        await SendMessage(message, queueName, properties, ct);
        using CancellationTokenRegistration ctr = ct.Register(() =>
        {
            _callbacks.TryRemove(properties.CorrelationId!, out _);
            callback.TrySetCanceled();
        });
        return await callback.Task;
    }

    private async Task<string> CreateTemporaryQueueName(CancellationToken ct = default)
    {
        QueueDeclareOk queue = await _channel.QueueDeclareAsync(cancellationToken: ct);
        return queue.QueueName;
    }

    private async Task<BasicProperties> GenerateCorellationProperties<TMessage>(
        string replyQueueName,
        TMessage message,
        CancellationToken ct
    )
    {
        await _channel.BasicConsumeAsync(
            queue: replyQueueName,
            autoAck: true,
            consumer: _consumer,
            cancellationToken: ct
        );
        CommunicationContextMessage<TMessage> corellationMessage = new(message);
        BasicProperties properties = new BasicProperties()
        {
            CorrelationId = corellationMessage.CorellationId,
            ReplyTo = replyQueueName,
        };
        return properties;
    }

    private TaskCompletionSource<TResponse?> RegisterReplyQueue(BasicProperties properties)
    {
        TaskCompletionSource<TResponse?> taskCompletionSource =
            new(TaskCreationOptions.RunContinuationsAsynchronously);
        _callbacks.TryAdd(properties.CorrelationId!, taskCompletionSource);
        return taskCompletionSource;
    }

    private async Task SendMessage<TMessage>(
        TMessage message,
        string targetQueueName,
        BasicProperties properties,
        CancellationToken ct = default
    )
    {
        string json = JsonSerializer.Serialize(message);
        byte[] body = Encoding.UTF8.GetBytes(json);

        _logger.Information(
            "Client send message: {Type} with body: {Json}",
            typeof(TMessage).Name,
            json
        );

        await _channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: targetQueueName,
            mandatory: true,
            basicProperties: properties,
            body: body,
            cancellationToken: ct
        );
    }

    private Task CreateCallbackListener(object model, BasicDeliverEventArgs ea)
    {
        string? corellationId = ea.BasicProperties.CorrelationId;
        if (string.IsNullOrWhiteSpace(corellationId))
            return Task.CompletedTask;

        if (!_callbacks.TryRemove(corellationId, out var taskCompletionSource))
            return Task.CompletedTask;

        byte[] body = ea.Body.ToArray();
        string json = Encoding.UTF8.GetString(body);

        _logger.Information("Client received response: {Json}", json);

        Type type = typeof(TResponse);
        if (type.Name == typeof(Result).Name)
        {
            Result resultValue = ResultJsonConverter.Convert(json);
            string tempJson = JsonSerializer.Serialize(resultValue);
            TResponse response = JsonSerializer.Deserialize<TResponse>(tempJson)!;
            taskCompletionSource.SetResult(response);
            return Task.CompletedTask;
        }

        TResponse? result = JsonSerializer.Deserialize<TResponse>(json);
        _logger.Information("Response is deserialized into: {Tresult}", result);
        taskCompletionSource.SetResult(result);
        return Task.CompletedTask;
    }

    public void Dispose() => _consumer.ReceivedAsync -= CreateCallbackListener;
}
