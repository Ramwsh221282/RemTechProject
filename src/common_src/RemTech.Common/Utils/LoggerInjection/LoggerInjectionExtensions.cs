using Microsoft.Extensions.DependencyInjection;
using RemTechCommon.Utils.DependencyInjectionHelpers;
using Serilog;

namespace RemTechCommon.Utils.LoggerInjection;

[DependencyInjection]
public static class LoggerInjectionExtensions
{
    [DependencyInjectionMethod]
    public static void Inject(this IServiceCollection services)
    {
        Serilog.ILogger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
        services.AddSingleton(logger);
    }
}
