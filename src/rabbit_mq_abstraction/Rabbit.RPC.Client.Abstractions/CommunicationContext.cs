﻿using System.Collections.Concurrent;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;

namespace Rabbit.RPC.Client.Abstractions;

internal sealed class CommunicationContext : IDisposable
{
    private readonly ConcurrentDictionary<
        string,
        TaskCompletionSource<ContractActionResult>
    > _callbacks = [];

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

    public async Task<ContractActionResult> Send<TMessage>(
        ContractRequest<TMessage> message,
        string queueName,
        CancellationToken ct = default
    )
        where TMessage : IContract
    {
        string replyQueueName = await CreateTemporaryQueueName(ct);
        BasicProperties properties = await GenerateCorellationProperties(replyQueueName, ct);
        TaskCompletionSource<ContractActionResult> callback = RegisterReplyQueue(properties);
        await SendMessage(message, queueName, properties, ct);

        using CancellationTokenRegistration ctr = ct.Register(() =>
        {
            _callbacks.TryRemove(properties.CorrelationId!, out _);
            callback.TrySetCanceled();
        });
        try
        {
            return await callback.Task;
        }
        catch (TaskCanceledException taskCancelled)
        {
            _logger.Information("Task cancelled. {Source}", taskCancelled.Source);
            return new ContractActionResult("Operation cancelled", false, new());
        }
        catch (OperationCanceledException operationCancelled)
        {
            _logger.Information("Operation cancelled. {Source}", operationCancelled.Source);
            return new ContractActionResult("Operation cancelled", false, new());
        }
    }

    private async Task<string> CreateTemporaryQueueName(CancellationToken ct = default)
    {
        QueueDeclareOk queue = await _channel.QueueDeclareAsync(cancellationToken: ct);
        return queue.QueueName;
    }

    private async Task<BasicProperties> GenerateCorellationProperties(
        string replyQueueName,
        CancellationToken ct
    )
    {
        await _channel.BasicConsumeAsync(
            queue: replyQueueName,
            autoAck: true,
            consumer: _consumer,
            cancellationToken: ct
        );
        BasicProperties properties = new()
        {
            CorrelationId = Guid.NewGuid().ToString(),
            ReplyTo = replyQueueName,
        };
        return properties;
    }

    private TaskCompletionSource<ContractActionResult> RegisterReplyQueue(
        BasicProperties properties
    )
    {
        TaskCompletionSource<ContractActionResult> taskCompletionSource = new(
            TaskCreationOptions.RunContinuationsAsynchronously
        );
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
        using MemoryStream memory = new MemoryStream();
        await JsonSerializer.SerializeAsync(memory, message, cancellationToken: ct);
        ReadOnlyMemory<byte> body = memory.GetBuffer().AsMemory(0, (int)memory.Length);

        await _channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: targetQueueName,
            mandatory: true,
            basicProperties: properties,
            body: body,
            cancellationToken: ct
        );

        _logger.Information("Client send message: {Type}", typeof(TMessage));
    }

    private async Task CreateCallbackListener(object model, BasicDeliverEventArgs ea)
    {
        try
        {
            string? corellationId = ea.BasicProperties.CorrelationId;
            if (string.IsNullOrWhiteSpace(corellationId))
                await Task.CompletedTask;

            if (!_callbacks.TryRemove(corellationId!, out var taskCompletionSource))
                await Task.CompletedTask;

            ContractActionResult result = JsonSerializer.Deserialize<ContractActionResult>(
                ea.Body.Span
            )!;

            taskCompletionSource!.SetResult(result);
            if (result.IsSuccess)
                _logger.Information(
                    "Client received response: IsSuccess: {IsSuccess}",
                    result.IsSuccess
                );
            else
                _logger.Error(
                    "Client received error response: IsSuccess: {IsSuccess} Error: {Error}",
                    result.IsSuccess,
                    result.Error.AsMemory()
                );
        }
        catch (TaskCanceledException ex)
        {
            _logger.Warning("{Context} task cancelled.", nameof(CommunicationContext));
        }
        catch (OperationCanceledException ex)
        {
            _logger.Warning("{Context} task cancelled.", nameof(CommunicationContext));
        }
        finally
        {
            await Task.CompletedTask;
        }
    }

    public void Dispose() => _consumer.ReceivedAsync -= CreateCallbackListener;
}
