using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using RemTech.Parser.Contracts.Contracts;
using RemTech.Parser.Contracts.Contracts.Commands;
using RemTech.Parser.Implementation.Commands.OpenPage;
using RemTech.Parser.Implementation.Core;

namespace RemTech.Parser.Implementation.Injection;

public sealed class WebDriverParserPluginInjection : IWebDriverInjection
{
    public IServiceCollection Inject(IServiceCollection services)
    {
        services.AddScoped<IValidator<OpenPageCommand>, OpenPageCommandValidator>();
        services.AddSingleton<WebDriverFactory>();
        services.AddSingleton<WebDriverExecutableManager>();
        services.AddSingleton<WebDriverInstanceOptions>();
        services.AddSingleton<WebDriverInstance>();
        services.AddSingleton<WebDriverDispatcher>();
        services.AddSingleton<IWebDriverApi, WebDriverApi>();

        InjectCommandHandlers(services);
        InjectHandlerValidators(services);
        InjectQueryHandlers(services);

        return services;
    }

    private void InjectCommandHandlers(IServiceCollection services) =>
        services.Scan(s =>
            s.FromAssemblies(typeof(WebDriverParserPluginInjection).Assembly)
                .AddClasses(classes => classes.AssignableTo(typeof(IWebDriverCommandHandler<>)))
                .AsSelfWithInterfaces()
                .WithScopedLifetime()
        );

    private void InjectQueryHandlers(IServiceCollection services) =>
        services.Scan(scan =>
            scan.FromAssemblies(typeof(WebDriverParserPluginInjection).Assembly)
                .AddClasses(classes => classes.AssignableTo(typeof(IWebDriverQueryHandler<,>)))
                .AsSelfWithInterfaces()
                .WithScopedLifetime()
        );

    private void InjectHandlerValidators(IServiceCollection services) =>
        services.Scan(s =>
            s.FromAssemblies(typeof(WebDriverParserPluginInjection).Assembly)
                .AddClasses(classes => classes.AssignableTo(typeof(AbstractValidator<>)))
                .AsSelfWithInterfaces()
                .WithScopedLifetime()
        );
}
