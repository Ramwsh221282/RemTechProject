using System.Text.Json;
using GuardValidationLibrary.Attributes;
using GuardValidationLibrary.GuardedFactory;
using MongoDB.Driver;
using RemTech.MongoDb.Service.Common.Guards;

namespace RemTech.MongoDb.Service.Configurations.MongoDbConfiguration;

public sealed class MongoDbConnectionOptions
{
    public string ConnectionString { get; }

    public MongoClient CreateClient() => new(ConnectionString);

    [GuardedConstructor]
    private MongoDbConnectionOptions(
        [GuardedParameter(typeof(StringNotEmptyGuard))] string connectionString
    ) => ConnectionString = connectionString;

    public static MongoDbConnectionOptions ReadMongoDbConnectionOptionsFile(string filePath)
    {
        string contents = File.ReadAllText(filePath);
        using JsonDocument document = JsonDocument.Parse(contents);

        bool hasConnectionString = document.RootElement.TryGetProperty(
            nameof(ConnectionString),
            out JsonElement connectionStringElement
        );

        if (!hasConnectionString)
            throw new ApplicationException("MongoDB connection string not found");

        string? connectionStringValue = connectionStringElement.GetString();
        if (string.IsNullOrWhiteSpace(connectionStringValue))
            throw new ApplicationException("Mongo Db Connection string has empty value");

        GuardedCreation<MongoDbConnectionOptions> optionsCreation =
            GuardedCreator.Create<MongoDbConnectionOptions>([connectionStringValue]);

        if (!optionsCreation.IsSuccess)
            throw new ApplicationException("Mongo Db Connection string has empty value");

        return optionsCreation.Object;
    }
}
