using Microsoft.Extensions.DependencyInjection;

namespace RemTechCommon.Injections;

public static class CommonInjections
{
    public static IServiceCollection AddCommonInjections(this IServiceCollection services)
    {
        services.InjectLogger();
        return services;
    }
}
