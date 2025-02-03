using Microsoft.Extensions.DependencyInjection;
using RemTech.Parser.Contracts.Contracts;
using RemTech.Parser.Implementation.Core;

namespace RemTech.Parser.Implementation.Injection;

public static class WebDriverParserPluginInjection
{
    public static IServiceCollection InjectWebDriverComponents(this IServiceCollection services)
    {
        services.AddTransient<IWebDriverFactory, WebDriverFactory>();
        services.AddSingleton<IWebDriverExecutableManager, WebDriverExecutableManager>();
        return services;
    }
}
