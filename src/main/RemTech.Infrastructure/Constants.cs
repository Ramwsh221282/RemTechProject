namespace RemTech.Infrastructure;

public static class Constants
{
    private static readonly string ConfigurationFolderPath = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        "Configurations"
    );

    public static readonly string PostgreSqlFilePath = Path.Combine(
        ConfigurationFolderPath,
        "POSTGRE_SQL_CONFIG.json"
    );
}
