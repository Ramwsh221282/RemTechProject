using System.Text.Json;
using RemTechCommon.Utils.ResultPattern;

namespace AvitoParser.PDK.Sinking.Configurations;

public sealed class SinkingClientConfiguration
{
    public string SinkToQueue { get; }
    public string SinkToHostName { get; }
    public string UserName { get; }
    public string Password { get; }

    private SinkingClientConfiguration(
        string sinkToQueue,
        string sinkToHostName,
        string userName,
        string password
    ) =>
        (SinkToQueue, SinkToHostName, UserName, Password) = (
            sinkToQueue,
            sinkToHostName,
            userName,
            password
        );

    public static class SinkingClientConfigurationLoader
    {
        public static SinkingClientConfiguration Load(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException(filePath);

            string contents = File.ReadAllText(filePath);
            if (string.IsNullOrWhiteSpace(contents))
                throw new InvalidDataException("Sinking client configuration file is empty.");

            using JsonDocument document = JsonDocument.Parse(contents);
            Result<string> sinkToQueue = ReadPropertyAsString(document, "SinkToQueue");

            if (sinkToQueue.IsFailure)
                throw new InvalidDataException("Sinking client configuration file is invalid.");

            Result<string> sinkToHostName = ReadPropertyAsString(document, "SinkToHostName");
            if (sinkToHostName.IsFailure)
                throw new InvalidDataException("Sinking client configuration file is invalid.");

            Result<string> userName = ReadPropertyAsString(document, "UserName");
            if (userName.IsFailure)
                throw new InvalidDataException("Sinking client configuration file is invalid.");

            Result<string> password = ReadPropertyAsString(document, "Password");
            if (password.IsFailure)
                throw new InvalidDataException("Sinking client configuration file is invalid.");

            return new SinkingClientConfiguration(sinkToQueue, sinkToHostName, userName, password);
        }

        private static Result<string> ReadPropertyAsString(
            JsonDocument document,
            string propertyName
        )
        {
            bool hasProperty = document.RootElement.TryGetProperty(
                propertyName,
                out JsonElement property
            );
            if (!hasProperty)
                return new Error($"Property: {propertyName} was not found");

            string? value = property.GetString();
            if (string.IsNullOrWhiteSpace(value))
                return new Error($"Property: {propertyName} was empty");

            return value;
        }
    }
}
