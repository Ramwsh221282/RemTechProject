using System;
using Microsoft.Extensions.DependencyInjection;
using SharedParsersLibrary.Attributes;

namespace SharedParsersLibrary.ParserBehaviorFacade;

[ParserDependencyInjection]
public static class ParserFacadeDependencyInjection
{
    [ParserDependencyInjectionMethod]
    public static void Inject(this IServiceCollection services)
    {
        services.AddSingleton<ParserManagementFacade>();
    }
}
