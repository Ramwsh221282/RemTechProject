namespace AvitoParser.PDK;

public sealed record PluginCommand
{
    public string PluginName { get; }
    public string PluginPath { get; }
    public AvitoPluginPayload? Payload { get; }

    public PluginCommand(string pluginName, string pluginPath, AvitoPluginPayload? payload = null)
    {
        PluginName = pluginName;
        PluginPath = pluginPath;
        Payload = payload;
    }

    public PluginCommand(string pluginName, AvitoPluginPayload? payload = null)
    {
        PluginName = pluginName;
        PluginPath = PluginPathBuilder.BuildPath(pluginName);
        Payload = payload;
    }
}
