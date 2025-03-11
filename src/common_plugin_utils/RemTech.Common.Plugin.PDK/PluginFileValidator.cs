using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTech.Common.Plugin.PDK;

public sealed record PluginFileValidator
{
    private readonly ILogger _logger;

    public PluginFileValidator(ILogger logger) => _logger = logger;

    public Result Validate(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            _logger.Error("{Context} plugin path is null or empty", nameof(PluginFileValidator));
            return new Error("Plugin path is null or empty");
        }

        if (!File.Exists(filePath))
        {
            _logger.Error(
                "{Context} no plugin with path: {Path}",
                nameof(PluginFileValidator),
                filePath
            );
            return new Error($"No plugin with path {filePath}");
        }

        if (filePath.EndsWith(".dll"))
            return Result.Success();

        _logger.Error("{Context} plugin file is invalid", nameof(PluginFileValidator));
        return new Error("Plugin file is invalid");
    }
}
