namespace RemTech.Common.Plugin.PDK;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class PluginAttribute : Attribute
{
    public string PluginName { get; set; } = string.Empty;
}
