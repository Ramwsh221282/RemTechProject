using Microsoft.Extensions.DependencyInjection;
using RemTechAvito.Application.Abstractions.Handlers;
using RemTechAvito.Application.FiltersManagement.CustomerStates.Commands.ParseCustomerStates;
using RemTechAvito.Application.FiltersManagement.CustomerTypes.Commands.ParseCustomerTypes;
using RemTechAvito.Application.FiltersManagement.TransportStates.Commands.ParseTransportStates;
using RemTechAvito.Application.FiltersManagement.TransportTypes.Commands.ParseTransportTypes;
using RemTechAvito.Application.TransportAdvertisementsManagement.TransportAdvertisements.Commands.ParseTransportAdvertisementsCatalogue;

namespace RemTechAvito.Application.Injection;

public static class ApplicationServicesExtensions
{
    public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<
            IAvitoCommandHandler<ParseTransportAdvertisementCatalogueCommand>,
            ParseTransportAdvertisementsCatalogueCommandHandler
        >();
        services.AddScoped<
            IAvitoCommandHandler<ParseCustomerStatesCommand>,
            ParseCustomerStatesCommandHandler
        >();
        services.AddScoped<
            IAvitoCommandHandler<ParseCustomerTypesCommand>,
            ParseCustomerTypesCommandHandler
        >();
        services.AddScoped<
            IAvitoCommandHandler<ParseTransportStatesCommand>,
            ParseTransportStatesCommandHandler
        >();
        services.AddScoped<
            IAvitoCommandHandler<ParseTransportTypesCommand>,
            ParseTransportTypesCommandHandler
        >();
        return services;
    }
}
