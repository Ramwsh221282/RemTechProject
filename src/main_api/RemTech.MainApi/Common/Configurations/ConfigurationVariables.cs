namespace RemTech.MainApi.Common.Configurations;

public static class ConfigurationVariables
{
    public static string ConfigurationFolderPath = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        "Configurations"
    );
}
