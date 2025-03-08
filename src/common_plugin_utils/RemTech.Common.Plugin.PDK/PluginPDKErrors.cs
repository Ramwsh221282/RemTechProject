using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTech.Common.Plugin.PDK;

public static class PluginPDKErrors
{
    public static Error PluginDependencyMissingError(string pluginName, string dependencyName) =>
        new Error($"{pluginName} missing dependency {dependencyName}");

    public static Error PluginDependencyMissingError(
        this ILogger logger,
        string pluginName,
        string dependencyName
    )
    {
        logger.Error("{Plugin} is missing dependency: {Dependency}", pluginName, dependencyName);
        return PluginDependencyMissingError(pluginName, dependencyName);
    }
}
