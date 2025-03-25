using RemTechCommon.Utils.ResultPattern;
using ILogger = Serilog.ILogger;

namespace RemTech.MainApi.Common.Extensions;

public static class LoggerExtensions
{
    public static void LogError(this ILogger logger, Error error, string context) =>
        logger.Error("{Context}: {Error}", context, error.Description);

    public static void LogFromContext(this ILogger logger, string message, string context) =>
        logger.Information("{Context}: {Message}", context, message);
}
