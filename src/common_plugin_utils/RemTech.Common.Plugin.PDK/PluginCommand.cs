namespace RemTech.Common.Plugin.PDK;

public sealed record PluginCommand
{
    public string PluginName { get; }
    public string PluginPath { get; }
    public PluginPayload? Payload { get; }

    public PluginCommand(string pluginName, string pluginPath, PluginPayload? payload = null)
    {
        PluginName = pluginName;
        PluginPath = pluginPath;
        Payload = payload;
    }

    public PluginCommand(string pluginName, PluginPayload? payload = null)
    {
        PluginName = pluginName;
        PluginPath = PluginPathBuilder.BuildPath(pluginName);
        Payload = payload;
    }
}
