namespace RemTechCommon.Constants;

public static class ApplicationPathConstants
{
    public static readonly string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
    public static readonly string PluginsDirectory = Path.Combine(CurrentDirectory, "Plugins");
}
