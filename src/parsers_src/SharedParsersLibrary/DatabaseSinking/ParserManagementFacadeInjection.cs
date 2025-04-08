using Microsoft.Extensions.DependencyInjection;
using SharedParsersLibrary.Attributes;

namespace SharedParsersLibrary.DatabaseSinking;

[ParserDependencyInjection]
public static class ParserManagementFacadeInjection
{
    [ParserDependencyInjectionMethod]
    public static void Inject(this IServiceCollection services)
    {
        services.AddSingleton<ParserManagementFacade>();
    }
}
