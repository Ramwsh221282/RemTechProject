using Serilog;

namespace RemTechAvito.Integrational.Tests.Helpers;

public static class LoggingHelper
{
    public static void LogTestFailedWithException(
        this ILogger logger,
        string testName,
        Exception ex
    )
    {
        logger.Fatal("{Test} failed. Error: {Message}", testName, ex.Message);
    }
}
