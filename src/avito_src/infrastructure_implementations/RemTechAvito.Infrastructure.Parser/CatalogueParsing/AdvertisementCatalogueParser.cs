using System.Runtime.CompilerServices;
using Rabbit.RPC.Client.Abstractions;
using RemTechAvito.Infrastructure.Contracts.Parser;
using RemTechAvito.Infrastructure.Contracts.Parser.AdvertisementsParsing;
using RemTechAvito.Infrastructure.Parser.CatalogueParsing.Models;
using RemTechAvito.Infrastructure.Parser.CatalogueParsing.Models.CustomBehaviors;
using Serilog;
using WebDriver.Worker.Service.Contracts.BaseImplementations;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

namespace RemTechAvito.Infrastructure.Parser.CatalogueParsing;

internal sealed class AdvertisementCatalogueParser : BaseParser, IAdvertisementCatalogueParser
{
    public AdvertisementCatalogueParser(IMessagePublisher publisher, ILogger logger)
        : base(publisher, logger) { }

    public async IAsyncEnumerable<ParsedTransportAdvertisement> Parse(
        string catalogueUrl,
        [EnumeratorCancellation] CancellationToken ct = default
    )
    {
        using WebDriverSession session = new(_publisher);
        await session.ExecuteBehavior(new StartBehavior("eager"), ct);

        CataloguePagination pagination = new(catalogueUrl, _publisher, _logger);
        await pagination.InitializePagination(ct);

        IEnumerable<string> cataloguePages = pagination.GetCataloguePageUrls();
        foreach (string page in cataloguePages)
        {
            CataloguePageModel model = new(page, _publisher, _logger);
            await model.Initialize(ct);

            IEnumerable<CatalogueItem> items = model.GetItems();
            foreach (CatalogueItem item in items)
            {
                AdvertisementParser parser = new(item, _publisher, _logger);
                await parser.Execute(ct);
                yield return item.ToParsed();
            }
        }
        await session.ExecuteBehavior(new StopBehavior(), ct);
    }
}
