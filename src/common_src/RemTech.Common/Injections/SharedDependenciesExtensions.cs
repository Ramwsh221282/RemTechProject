using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace RemTechCommon.Injections;

public static class SharedDependenciesExtensions
{
    public static IServiceCollection RegisterSharedDependencies(this IServiceCollection services)
    {
        ILogger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
        services.AddSingleton(logger);
        return services;
    }
}
