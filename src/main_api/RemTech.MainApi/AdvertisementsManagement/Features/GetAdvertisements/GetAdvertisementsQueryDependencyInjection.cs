using RemTech.MainApi.AdvertisementsManagement.Features.GetAdvertisements.Decorators;
using RemTech.MainApi.AdvertisementsManagement.Models;
using RemTech.MainApi.ParsersManagement.Messages;
using RemTechCommon.Utils.CqrsPattern;
using RemTechCommon.Utils.DependencyInjectionHelpers;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.MainApi.AdvertisementsManagement.Features.GetAdvertisements;

[DependencyInjection]
public static class GetAdvertisementsQueryDependencyInjection
{
    [DependencyInjectionMethod]
    public static void Inject(this IServiceCollection services)
    {
        services.AddScoped<
            IRequestHandler<GetAdvertisementsQuery, Result<(TransportAdvertisement[], long)>>
        >(p =>
        {
            Serilog.ILogger logger = p.GetRequiredService<Serilog.ILogger>();
            DataServiceMessagerFactory factory = p.GetRequiredService<DataServiceMessagerFactory>();
            DataServiceMessager messager = factory.Create();
            GetAdvertisementsQueryHandler h1 = new(messager);
            GetAdvertisementsQueryValidationDecorator h2 = new(h1);
            GetAdvertisementsQueryLoggingDecorator h3 = new(h2, logger);
            return h3;
        });
    }
}
