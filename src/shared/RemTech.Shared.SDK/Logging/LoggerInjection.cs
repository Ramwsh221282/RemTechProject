using Microsoft.Extensions.DependencyInjection;
using RemTech.Shared.SDK.DependencyInjection;
using Serilog;

namespace RemTech.Shared.SDK.Logging;

[InjectionClass]
public static class LoggerInjection
{
    [InjectionMethod]
    public static void Inject(this IServiceCollection services)
    {
        ILogger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
        services.AddSingleton(logger);
    }
}
