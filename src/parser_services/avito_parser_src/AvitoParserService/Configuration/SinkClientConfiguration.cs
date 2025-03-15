using System.Text.Json;

namespace AvitoParserService.Configuration;

public sealed class SinkClientConfiguration
{
    public string QueueName { get; }
    public string HostName { get; }
    public string UserName { get; }
    public string Password { get; }

    private SinkClientConfiguration(
        string queueName,
        string hostName,
        string userName,
        string password
    ) => (QueueName, HostName, UserName, Password) = (queueName, hostName, userName, password);

    public static class SinkConfigurationReader
    {
        public static SinkClientConfiguration Read(string filePath)
        {
            string contents = File.ReadAllText(filePath);
            using JsonDocument document = JsonDocument.Parse(contents);
            bool hasQueueName = document.RootElement.TryGetProperty(
                "QueueName",
                out JsonElement queueNameElement
            );
            bool hasHostName = document.RootElement.TryGetProperty(
                "HostName",
                out JsonElement hostNameElement
            );
            bool hasUserName = document.RootElement.TryGetProperty(
                "UserName",
                out JsonElement userNameElement
            );
            bool hasPassword = document.RootElement.TryGetProperty(
                "Password",
                out JsonElement passwordElement
            );

            if (!hasQueueName || !hasHostName || !hasUserName || !hasPassword)
                throw new ApplicationException(
                    "Service has no  QueueName, HostName, UserName, Password in sink configuration"
                );

            string? queueName = queueNameElement.GetString();
            string? hostName = hostNameElement.GetString();
            string? userName = userNameElement.GetString();
            string? password = passwordElement.GetString();
            string?[] items = [queueName, hostName, userName, password];
            if (items.Any(string.IsNullOrWhiteSpace))
                throw new ApplicationException(
                    "Service has no  QueueName, HostName, UserName, Password in sink configuration"
                );

            return new SinkClientConfiguration(queueName!, hostName!, userName!, password!);
        }
    }
}
