using RemTech.MainApi.AdvertisementsManagement.Features.GetAdvertisements.Decorators;
using RemTech.MainApi.AdvertisementsManagement.Models;
using RemTech.MainApi.Common.Abstractions;
using RemTech.MainApi.Common.Attributes;
using RemTech.MainApi.ParsersManagement.Messages;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.MainApi.AdvertisementsManagement.Features.GetAdvertisements;

[DependencyInjection]
public static class GetAdvertisementsQueryDependencyInjection
{
    [ServicesRegistration]
    public static void Inject(this IServiceCollection services)
    {
        services.AddScoped<IQueryHandler<GetAdvertisementsQuery, Result<TransportAdvertisement[]>>>(
            p =>
            {
                Serilog.ILogger logger = p.GetRequiredService<Serilog.ILogger>();
                DataServiceMessagerFactory factory =
                    p.GetRequiredService<DataServiceMessagerFactory>();
                DataServiceMessager messager = factory.Create();
                GetAdvertisementsQueryHandler h1 = new(messager);
                GetAdvertisementsQueryValidationDecorator h2 = new(h1);
                GetAdvertisementsQueryLoggingDecorator h3 = new(h2, logger);
                return h3;
            }
        );
    }
}
