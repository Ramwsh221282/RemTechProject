using Microsoft.Extensions.DependencyInjection;
using RemTechAvito.Application.Injection;
using RemTechAvito.Infrastructure.Parser.Injection;
using RemTechAvito.Infrastructure.Repository.Injection;
using RemTechCommon.Injections;

namespace RemTechAvito.DependencyInjection;

public static class RemTechAvitoDependencyInjection
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.RegisterSharedDependencies();
        services.RegisterParser();
        services.RegisterRepositories();
        services.RegisterApplicationServices();
        return services;
    }
}
