using System.Text.Json;

namespace SharedParsersLibrary.Configuration;

public sealed class ScrapedAdvertisementsSinkerConfigurationFactory
{
    private static readonly string ConfigurationFolder = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        "SinkerConfiguration"
    );

    private static readonly string ConfigurationFile = Path.Combine(
        ConfigurationFolder,
        "Sinker.json"
    );

    public ScrapedAdvertisementsSinkerConfiguration Create()
    {
        CheckFolder();
        CheckFile();
        string contents = File.ReadAllText(ConfigurationFile);
        CheckContents(contents);
        using JsonDocument document = JsonDocument.Parse(contents);
        bool hasQueueName = document.RootElement.TryGetProperty(
            "QueueName",
            out JsonElement queueName
        );
        bool hasHostName = document.RootElement.TryGetProperty(
            "HostName",
            out JsonElement hostName
        );
        bool hasPassword = document.RootElement.TryGetProperty(
            "Password",
            out JsonElement password
        );
        bool hasUsername = document.RootElement.TryGetProperty(
            "UserName",
            out JsonElement username
        );

        if (!hasQueueName || !hasHostName || !hasPassword || !hasUsername)
            throw new InvalidDataException("Invalid sinker configuration file json");

        string? queueNameValue = queueName.GetString();
        string? hostNameValue = hostName.GetString();
        string? passwordValue = password.GetString();
        string? usernameValue = username.GetString();

        if (
            string.IsNullOrWhiteSpace(queueNameValue)
            || string.IsNullOrWhiteSpace(hostNameValue)
            || string.IsNullOrWhiteSpace(passwordValue)
            || string.IsNullOrWhiteSpace(usernameValue)
        )
            throw new InvalidDataException("Invalid sinker configuration file json");

        return new ScrapedAdvertisementsSinkerConfiguration(
            queueNameValue,
            hostNameValue,
            usernameValue,
            passwordValue
        );
    }

    private void CheckFolder()
    {
        if (!Directory.Exists(ConfigurationFolder))
            throw new FileNotFoundException(
                "Sinker configuration folder not found",
                ConfigurationFolder
            );
    }

    private void CheckFile()
    {
        if (!File.Exists(ConfigurationFile))
            throw new FileNotFoundException(
                "Sinker configuration file not found",
                ConfigurationFile
            );
    }

    private void CheckContents(string contents)
    {
        if (string.IsNullOrWhiteSpace(contents))
            throw new InvalidDataException("Sinker configuration content is empty.");
    }
}
