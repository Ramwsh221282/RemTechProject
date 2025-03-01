using System.Runtime.CompilerServices;
using Rabbit.RPC.Client.Abstractions;
using RemTechAvito.Infrastructure.Contracts.Parser.AdvertisementsParsing;
using RemTechAvito.Infrastructure.Parser.CatalogueParsing.Models;
using RemTechAvito.Infrastructure.Parser.CatalogueParsing.Models.CustomBehaviors;
using Serilog;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

namespace RemTechAvito.Infrastructure.Parser.CatalogueParsing;

internal sealed class AdvertisementCatalogueParser : BaseParser, IAdvertisementCatalogueParser
{
    public AdvertisementCatalogueParser(IMessagePublisher publisher, ILogger logger)
        : base(publisher, logger) { }

    public async IAsyncEnumerable<ParsedTransportAdvertisement> Parse(
        string catalogueUrl,
        IEnumerable<string>? additions = null,
        IEnumerable<long>? existingIds = null,
        [EnumeratorCancellation] CancellationToken ct = default
    )
    {
        var start = new StartBehavior("none");
        var starting = await start.Execute(_publisher, ct);
        if (starting.IsFailure)
        {
            _logger.Error("{Parser} cannot start", nameof(AdvertisementCatalogueParser));
            yield break;
        }

        var openPage = new OpenPageBehavior(catalogueUrl);
        var opening = await openPage.Execute(_publisher, ct);
        if (opening.IsFailure)
        {
            _logger.Error(
                "{Parser} cannot open page: {url}",
                nameof(AdvertisementCatalogueParser),
                catalogueUrl
            );
            yield break;
        }

        var bottom = new ScrollToBottomRetriable(10);
        await bottom.Execute(_publisher, ct);
        var top = new ScrollToTopRetriable(10);
        await top.Execute(_publisher, ct);

        CataloguePagination pagination = new(catalogueUrl, _publisher, _logger);
        await pagination.InitializePagination(ct);

        var cataloguePages = pagination.GetCataloguePageUrls();
        foreach (var page in cataloguePages)
        {
            CataloguePageModel model = new(page, _publisher, _logger);
            await model.Initialize(ct);

            var items = model.GetItems();
            foreach (var item in items)
            {
                AdvertisementParser parser = new(item, _publisher, _logger);
                await parser.Execute(ct);
                var parsed = item.ToParsed();

                if (additions != null && additions.Any())
                {
                    var isMatch = parsed.IsFollowingAdditions(additions);
                    if (isMatch)
                        yield return parsed;

                    continue;
                }

                yield return item.ToParsed();
            }
        }

        var stopBehavior = new StopBehavior();
        await stopBehavior.Execute(_publisher, ct);
    }
}
