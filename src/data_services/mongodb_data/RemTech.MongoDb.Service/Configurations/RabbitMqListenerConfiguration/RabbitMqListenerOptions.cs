using System.Text.Json;
using GuardValidationLibrary.Attributes;
using GuardValidationLibrary.GuardedFactory;
using Rabbit.RPC.Server.Abstractions.Core;
using RemTech.MongoDb.Service.Common.Guards;

namespace RemTech.MongoDb.Service.Configurations.RabbitMqListenerConfiguration;

public sealed class RabbitMqListenerOptions : IRabbitMqListenerOptions
{
    public string QueueName { get; }
    public string HostName { get; }
    public string UserName { get; }
    public string Password { get; }

    [GuardedConstructor]
    private RabbitMqListenerOptions(
        [GuardedParameter(typeof(StringNotEmptyGuard))] string queueName,
        [GuardedParameter(typeof(StringNotEmptyGuard))] string hostName,
        [GuardedParameter(typeof(StringNotEmptyGuard))] string userName,
        [GuardedParameter(typeof(StringNotEmptyGuard))] string password
    ) => (QueueName, HostName, UserName, Password) = (queueName, hostName, userName, password);

    public static RabbitMqListenerOptions ReadRabbitMqListenerOptionsFile(string filePath)
    {
        string contents = File.ReadAllText(filePath);
        using JsonDocument document = JsonDocument.Parse(contents);

        bool hasQueueName = document.RootElement.TryGetProperty(
            nameof(QueueName),
            out JsonElement queueNameElement
        );
        bool hasHostName = document.RootElement.TryGetProperty(
            nameof(HostName),
            out JsonElement hostNameElement
        );
        bool hasUserName = document.RootElement.TryGetProperty(
            nameof(UserName),
            out JsonElement userNameElement
        );
        bool hasPassword = document.RootElement.TryGetProperty(
            nameof(Password),
            out JsonElement passwordElement
        );

        if (!hasQueueName || !hasHostName || !hasUserName || !hasPassword)
            throw new ApplicationException("Invalid rabbit mq listener configuration file");

        string? queueName = queueNameElement.GetString();
        string? hostName = hostNameElement.GetString();
        string? userName = userNameElement.GetString();
        string? password = passwordElement.GetString();

        GuardedCreation<RabbitMqListenerOptions> creation =
            GuardedCreator.Create<RabbitMqListenerOptions>(
                queueName!,
                hostName!,
                userName!,
                password!
            );

        if (!creation.IsSuccess)
            throw new ApplicationException("Invalid rabbit mq listener configuration file");

        return creation.Object;
    }
}
