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

        services.Scan(s =>
            s.FromAssemblies(typeof(WebDriverParserPluginInjection).Assembly)
                .AddClasses(classes => classes.AssignableTo(typeof(IWebDriverCommandHandler<>)))
                .AsSelfWithInterfaces()
                .WithScopedLifetime()
        );
        services.Scan(s =>
            s.FromAssemblies(typeof(WebDriverParserPluginInjection).Assembly)
                .AddClasses(classes => classes.AssignableTo(typeof(AbstractValidator<>)))
                .AsSelfWithInterfaces()
                .WithScopedLifetime()
        );

        services.AddSingleton<IWebDriverApi, WebDriverApi>();
        return services;
    }
}
