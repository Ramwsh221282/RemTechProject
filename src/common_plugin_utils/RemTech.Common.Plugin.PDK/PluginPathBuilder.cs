using System.Text;

namespace RemTech.Common.Plugin.PDK;

public static class PluginPathBuilder
{
    public static string BuildPath(string pluginName)
    {
        StringBuilder stringBuilder = new StringBuilder(pluginName);
        stringBuilder.Append(".dll");
        string fileName = stringBuilder.ToString();
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins", fileName);
        return path;
    }
}
