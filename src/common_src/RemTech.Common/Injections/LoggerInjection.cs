using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace RemTechCommon.Injections;

public static class LoggerInjection
{
    public static IServiceCollection InjectLogger(this IServiceCollection services)
    {
        ILogger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
        services.AddSingleton(logger);
        return services;
    }
}
