using Serilog;

namespace RemTech.Shared.SDK.Logging;

public static class LoggerExtensions
{
    private const string LogTemplate = "{Context} {Action} {Message}";

    public static void LogError(
        this ILogger logger,
        string context,
        string action,
        string message
    ) => logger.Error(LogTemplate, context, action, message);

    public static void LogFatal(
        this ILogger logger,
        string context,
        string action,
        string message
    ) => logger.Fatal(LogTemplate, context, action, message);

    public static void LogInfo(
        this ILogger logger,
        string context,
        string action,
        string message
    ) => logger.Information(LogTemplate, context, action, message);
}
