using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using WebDriver.Core.Core;

namespace WebDriver.Core.Injection;

public static class WebDriverPluginLoader
{
    public static void RegisterWebDriverServices(this IServiceCollection services)
    {
        services.RegisterWebDriverDependencies();
        services.RegisterHandlers();
    }

    private static void RegisterHandlers(this IServiceCollection services)
    {
        services.Scan(x =>
            x.FromAssemblies(typeof(WebDriverPluginLoader).Assembly)
                .AddClasses(classess => classess.AssignableTo(typeof(IWebDriverCommandHandler<>)))
                .AsSelfWithInterfaces()
                .WithScopedLifetime()
                .AddClasses(classes => classes.AssignableTo(typeof(IWebDriverQueryHandler<,>)))
                .AsSelfWithInterfaces()
                .WithScopedLifetime()
                .AddClasses(classes => classes.AssignableTo(typeof(AbstractValidator<>)))
                .AsSelfWithInterfaces()
                .WithScopedLifetime()
        );
    }

    private static void RegisterWebDriverDependencies(this IServiceCollection services)
    {
        services.AddSingleton<WebDriverFactory>();
        services.AddSingleton<WebDriverExecutableManager>();
        services.AddSingleton<WebDriverInstanceOptions>();
        services.AddSingleton<WebDriverInstance>();
        services.AddSingleton<WebDriverDispatcher>();
        services.AddSingleton<WebDriverApi>();
    }
}
