using System.Text.Json;
using RemTech.MainApi.Common.Configurations;

namespace RemTech.MainApi.ParsersManagement.Configurations;

public class ParserDataServiceConfiguration
{
    private static string ParserDataServiceConfigurationFolder = Path.Combine(
        ConfigurationVariables.ConfigurationFolderPath,
        "ParserEndpointConfigurations"
    );

    private static string ParserDataServiceMessagerConfigurationFilePath = Path.Combine(
        ParserDataServiceConfigurationFolder,
        "PARSER_DATA_SERVICE_MESSAGER_CONFIG.json"
    );

    public string QueueName { get; }
    public string HostName { get; }
    public string UserName { get; }
    public string Password { get; }

    private ParserDataServiceConfiguration(
        string queueName,
        string hostName,
        string userName,
        string password
    ) => (QueueName, HostName, UserName, Password) = (queueName, hostName, userName, password);

    public static ParserDataServiceConfiguration Create()
    {
        if (!Directory.Exists(ParserDataServiceConfigurationFolder))
            throw new ApplicationException(
                "Parser Data Service Configuration folder doesn't exist."
            );

        if (!File.Exists(ParserDataServiceMessagerConfigurationFilePath))
            throw new ApplicationException(
                "Parser Data Service Messager configuration file doesn't exist."
            );

        string contents = File.ReadAllText(ParserDataServiceMessagerConfigurationFilePath);
        if (string.IsNullOrWhiteSpace(contents))
            throw new ApplicationException(
                "Parser Data Service Messager configuration file is empty."
            );

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

        if (!hasQueueName && !hasHostName && !hasUserName && !hasPassword)
            throw new ApplicationException(
                "Parser Data Service Messager configuration file is not valid."
            );

        string? queueNameValue = queueNameElement.GetString();
        string? hostNameValue = hostNameElement.GetString();
        string? userNameValue = userNameElement.GetString();
        string? passwordValue = passwordElement.GetString();
        string?[] properties = [queueNameValue, hostNameValue, userNameValue, passwordValue];

        if (properties.Any(string.IsNullOrWhiteSpace))
            throw new ApplicationException(
                "Parser Data Service Messager configuration file is not valid."
            );

        return new ParserDataServiceConfiguration(
            queueNameValue!,
            hostNameValue!,
            userNameValue!,
            passwordValue!
        );
    }
}
