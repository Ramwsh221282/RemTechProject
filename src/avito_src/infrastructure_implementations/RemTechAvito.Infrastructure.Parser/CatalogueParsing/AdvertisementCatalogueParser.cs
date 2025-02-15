using Rabbit.RPC.Client.Abstractions;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement;
using RemTechAvito.Infrastructure.Contracts.Parser;
using RemTechAvito.Infrastructure.Parser.CatalogueParsing.Models;
using RemTechAvito.Infrastructure.Parser.CatalogueParsing.Models.CustomBehaviors;
using Serilog;
using WebDriver.Worker.Service.Contracts.BaseImplementations;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

namespace RemTechAvito.Infrastructure.Parser.CatalogueParsing;

public sealed class AdvertisementCatalogueParser : BaseParser, IAdvertisementCatalogueParser
{
    public AdvertisementCatalogueParser(IMessagePublisher publisher, ILogger logger)
        : base(publisher, logger) { }

    public async Task<IReadOnlyCollection<TransportAdvertisement>> Parse(
        string catalogueUrl,
        CancellationToken ct = default
    )
    {
        CataloguePageModel model = new CataloguePageModel(catalogueUrl);
        using WebDriverSession session = new WebDriverSession(_publisher);
        await session.ExecuteBehavior(new StartBehavior("none"));
        await session.ExecuteBehavior(
            new InitializePaginationBehavior(model, _logger, catalogueUrl)
        );
        await session.ExecuteBehavior(new ParseAvitoPages(model, _logger));
        await session.ExecuteBehavior(new ParseAdvertisements(model));
        await session.ExecuteBehavior(new StopBehavior(), ct);
        return [];
    }
}
