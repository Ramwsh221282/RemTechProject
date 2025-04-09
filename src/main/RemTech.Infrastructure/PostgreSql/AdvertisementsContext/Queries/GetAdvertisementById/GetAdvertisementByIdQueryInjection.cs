using Microsoft.Extensions.DependencyInjection;
using RemTech.Infrastructure.PostgreSql.AdvertisementsContext.DaoModels;
using RemTech.Shared.SDK.CqrsPattern.Queries;
using RemTech.Shared.SDK.DependencyInjection;
using RemTech.Shared.SDK.OptionPattern;

namespace RemTech.Infrastructure.PostgreSql.AdvertisementsContext.Queries.GetAdvertisementById;

[InjectionClass]
public static class GetAdvertisementByIdQueryInjection
{
    [InjectionMethod]
    public static void Inject(this IServiceCollection services)
    {
        services.AddScoped<
            IQueryHandler<GetAdvertisementByIdQuery, Option<AdvertisementDao>>,
            GetAdvertisementByIdQueryHandler
        >();
    }
}
