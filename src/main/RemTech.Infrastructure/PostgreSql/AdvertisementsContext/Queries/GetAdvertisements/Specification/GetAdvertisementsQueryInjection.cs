using Microsoft.Extensions.DependencyInjection;
using RemTech.Infrastructure.PostgreSql.AdvertisementsContext.DaoModels;
using RemTech.Shared.SDK.CqrsPattern.Queries;
using RemTech.Shared.SDK.DependencyInjection;

namespace RemTech.Infrastructure.PostgreSql.AdvertisementsContext.Queries.GetAdvertisements.Specification;

[InjectionClass]
public static class GetAdvertisementsQueryInjection
{
    [InjectionMethod]
    public static void Inject(this IServiceCollection services)
    {
        services.AddScoped<
            IQueryHandler<GetAdvertisementsQuery, AdvertisementDao[]>,
            GetAdvertisementsQueryHandler
        >();
    }
}
