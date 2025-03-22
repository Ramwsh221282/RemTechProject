namespace RemTech.MongoDb.Service.Configurations;

public static class ConfigurationVariables
{
    public static readonly string ConfigurationFolderPath = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        "Configurations"
    );

    public static readonly string MongoDbConfigFilePath = Path.Combine(
        ConfigurationFolderPath,
        "MONGO_DB_CONFIG.json"
    );

    public static readonly string RabbitMqListenerConfigFilePath = Path.Combine(
        ConfigurationFolderPath,
        "SERVICE_CONFIG.json"
    );

    public static void ValidateConfigurationFilesExistance()
    {
        if (!Directory.Exists(ConfigurationFolderPath))
            throw new ApplicationException("Missing Configurations folder");

        if (!File.Exists(MongoDbConfigFilePath))
            throw new ApplicationException("Missing MongoDB config file");

        if (!File.Exists(RabbitMqListenerConfigFilePath))
            throw new ApplicationException("Missing RabbitMQ listener config file");
    }
}
