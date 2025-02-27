using Microsoft.Extensions.DependencyInjection;
using RemTechAvito.Application.Abstractions.Handlers;
using RemTechAvito.Application.FiltersManagement.CustomerStates.Commands.ParseCustomerStates;
using RemTechAvito.Application.FiltersManagement.CustomerTypes.Commands.ParseCustomerTypes;
using RemTechAvito.Application.FiltersManagement.TransportStates.Commands.ParseTransportStates;
using RemTechAvito.Application.FiltersManagement.TransportTypes.Commands.CreateCustomTransportType;
using RemTechAvito.Application.FiltersManagement.TransportTypes.Commands.ParseTransportTypes;
using RemTechAvito.Application.ParserProfileManagement.CreateProfile;
using RemTechAvito.Application.ParserProfileManagement.DeleteProfile;
using RemTechAvito.Application.ParserProfileManagement.UpdateParserProfileLinks;
using RemTechAvito.Application.TransportAdvertisementsManagement.TransportAdvertisements.Commands.ParseTransportAdvertisementsCatalogue;
using RemTechAvito.Contracts.Common.Responses.ParserProfileManagement;
using RemTechAvito.Contracts.Common.Responses.TransportTypesManagement;

namespace RemTechAvito.Application.Injection;

public static class ApplicationServicesExtensions
{
    public static void RegisterApplicationServices(this IServiceCollection services)
    {
        services.RegisterTransportAdvertisementsManagement();
        services.RegisterFilterManagement();
        services.RegisterParserProfileManagement();
    }

    private static void RegisterFilterManagement(this IServiceCollection services)
    {
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
        CreateCustomTransportTypeCommand.Register(services);
        ParseTransportTypesCommand.Register(services);
    }

    private static void RegisterTransportAdvertisementsManagement(this IServiceCollection services)
    {
        services.AddScoped<
            IAvitoCommandHandler<ParseTransportAdvertisementCatalogueCommand>,
            ParseTransportAdvertisementsCatalogueCommandHandler
        >();
    }

    private static void RegisterParserProfileManagement(this IServiceCollection services)
    {
        services.AddScoped<
            IAvitoCommandHandler<CreateProfileCommand, ParserProfileResponse>,
            CreateProfileCommandHandler
        >();
        services.AddScoped<
            IAvitoCommandHandler<UpdateParserProfileLinksCommand>,
            UpdateParserProfileLinksCommandHandler
        >();
        services.AddScoped<
            IAvitoCommandHandler<DeleteParserProfileCommand>,
            DeleteParserProfileCommandHandler
        >();
    }
}
