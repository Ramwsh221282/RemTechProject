using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using RemTech.WebDriver.Plugin.Core;
using WebDriver.Core.Core;

namespace WebDriver.Core.Injection;

public sealed class WebDriverPluginLoader
{
    public IServiceCollection RegisterServices(IServiceCollection services)
    {
        services = RegisterHandlers(services);
        services = RegisterWebDriverDependencies(services);
        return services;
    }

    private IServiceCollection RegisterHandlers(IServiceCollection services)
    {
        services.Scan(x =>
            x.FromAssemblyOf<WebDriverPluginLoader>()
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
        return services;
    }

    private IServiceCollection RegisterWebDriverDependencies(IServiceCollection services)
    {
        services.AddSingleton<WebDriverFactory>();
        services.AddSingleton<WebDriverExecutableManager>();
        services.AddSingleton<WebDriverInstanceOptions>();
        services.AddSingleton<WebDriverInstance>();
        services.AddSingleton<WebDriverDispatcher>();
        services.AddSingleton<WebDriverApi>();
        return services;
    }
}
