using Microsoft.Extensions.DependencyInjection;
using Rabbit.RPC.Client.Abstractions;
using RemTechAvito.Infrastructure.Contracts.Parser.AdvertisementsParsing;
using RemTechAvito.Infrastructure.Contracts.Parser.FiltersParsing;
using RemTechAvito.Infrastructure.Parser.CatalogueParsing;

namespace RemTechAvito.Infrastructure.Parser.Injection;

public static class ParserExtensions
{
    // TEMPORARY WILL BE REMOVED.
    private const string host = "localhost";
    private const string user = "rmuser";
    private const string password = "rmpassword";
    private const string queue = "web-driver-service";

    public static IServiceCollection RegisterParser(this IServiceCollection services)
    {
        services.AddScoped<IMessagePublisher, MultiCommunicationPublisher>(
            _ => new MultiCommunicationPublisher(queue, host, user, password)
        );
        services.AddScoped<ITransportTypesParser, TransportTypesParser>();
        services.AddScoped<IAdvertisementCatalogueParser, AdvertisementCatalogueParser>();
        return services;
    }
}
