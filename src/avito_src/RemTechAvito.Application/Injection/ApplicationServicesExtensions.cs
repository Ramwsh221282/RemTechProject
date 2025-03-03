using Microsoft.Extensions.DependencyInjection;
using RemTechAvito.Application.ParserProfileManagement.CreateProfile;
using RemTechAvito.Application.ParserProfileManagement.DeleteProfile;
using RemTechAvito.Application.ParserProfileManagement.UpdateProfile;
using RemTechAvito.Application.TransportAdvertisementsManagement.TransportAdvertisements.Commands.CreateCustomTransportType;
using RemTechAvito.Application.TransportAdvertisementsManagement.TransportAdvertisements.Commands.ParseTransportAdvertisementsCatalogue;
using RemTechAvito.Application.TransportAdvertisementsManagement.TransportAdvertisements.Commands.DeleteTransportType;
using RemTechAvito.Application.TransportAdvertisementsManagement.TransportAdvertisements.Commands.ParseTransportTypes;

namespace RemTechAvito.Application.Injection;

public static class ApplicationServicesExtensions
{
    public static void RegisterApplicationServices(this IServiceCollection services)
    {
        services.RegisterProfileManagement();
        services.RegisterAdvertisementsManagement();
    }

    private static void RegisterAdvertisementsManagement(this IServiceCollection services)
    {
        CreateCustomTransportTypeCommand.Register(services);
        ParseTransportAdvertisementCatalogueCommand.Register(services);
        ParseTransportTypesCommand.Register(services);
        DeleteTransportTypeCommand.Register(services);
    }

    private static void RegisterProfileManagement(this IServiceCollection services)
    {
        CreateProfileCommand.Register(services);
        DeleteParserProfileCommand.Register(services);
        UpdateParserProfileCommand.Register(services);
    }
}
