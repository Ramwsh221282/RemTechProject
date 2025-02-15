using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Worker.Service.Contracts.BaseImplementations;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

namespace RemTechAvito.Infrastructure.Parser.CatalogueParsing.Models.CustomBehaviors;

internal sealed class ParseCataloguePageBehavior : IWebDriverBehavior
{
    private readonly CataloguePageModel _model;
    private readonly ILogger _logger;
    private const string pathType = "xpath";
    private const string cataloguePath = ".//div[@data-marker='catalog-serp']";
    private const string catalogue = "catalogue";
    private const string itemPath = ".//div[@data-marker='item']";
    private const string item = "item";
    private const string itemLinkPath = ".//a[@itemprop='url']";
    private const string link = "link";
    private const string linkAttribute = "href";

    public ParseCataloguePageBehavior(CataloguePageModel model, ILogger logger)
    {
        _model = model;
        _logger = logger;
    }

    public async Task<Result> Execute(IMessagePublisher publisher, CancellationToken ct = default)
    {
        if (_model.IsReachedMaxPage)
            return Result.Success();

        WebElementPool pool = new WebElementPool();
        await CreateItemsInPool(pool, publisher, ct);
        await InitializeItems(pool, publisher, ct);
        await InitializeUrlsOfItems(pool, publisher, ct);
        AppendCatalogueItemsInModel(pool);
        ClearPoolBehavior clear = new ClearPoolBehavior();
        await clear.Execute(publisher, ct);
        return Result.Success();
    }

    private async Task CreateItemsInPool(
        WebElementPool pool,
        IMessagePublisher publisher,
        CancellationToken ct = default
    )
    {
        CompositeBehavior pipeline = new CompositeBehavior()
            .AddBehavior(new ScrollToBottomRetriable(10))
            .AddBehavior(new ScrollToTopRetriable(10))
            .AddBehavior(new GetNewElementRetriable(pool, cataloguePath, pathType, catalogue, 10))
            .AddBehavior(
                new DoForLastParent(
                    pool,
                    element => new GetChildrenBehavior(element, item, itemPath, pathType)
                )
            )
            .AddBehavior(
                new DoForAllChildren(
                    pool,
                    catalogue,
                    element => new GetChildrenBehavior(element, link, itemLinkPath, linkAttribute)
                )
            );

        await pipeline.Execute(publisher, ct);
    }

    private async Task InitializeItems(
        WebElementPool pool,
        IMessagePublisher publisher,
        CancellationToken ct = default
    )
    {
        if (pool.Count == 0)
        {
            _logger.Warning(
                "{Class} Avito catalogue container was not found",
                nameof(ParseAvitoPages)
            );
            return;
        }

        Result<WebElement> container = pool[pool.Count - 1];
        if (container.IsFailure)
        {
            _logger.Warning(
                "{Class} Avito catalogue container was not found",
                nameof(ParseAvitoPages)
            );
            return;
        }

        foreach (var child in container.Value.Childs)
        {
            GetSingleChildAsParent creatingItems = new GetSingleChildAsParent(
                child,
                pool,
                itemLinkPath,
                pathType,
                "link-item"
            );
            await creatingItems.Execute(publisher, ct);
        }
    }

    private async Task InitializeUrlsOfItems(
        WebElementPool pool,
        IMessagePublisher publisher,
        CancellationToken ct = default
    )
    {
        if (pool.Count == 0)
        {
            _logger.Warning(
                "{Class} Avito catalogue container was not found",
                nameof(ParseAvitoPages)
            );
            return;
        }

        for (int index = 1; index < pool.Count; index++)
        {
            InitializeAttributeRepeatable initialization = new(pool[index], linkAttribute, 10);
            await initialization.Execute(publisher, ct);
        }
    }

    private void AppendCatalogueItemsInModel(WebElementPool pool)
    {
        if (pool.Count == 0 || pool.Count == 1)
        {
            _logger.Warning(
                "{Class} Avito catalogue items were not found",
                nameof(ParseAvitoPages)
            );
            return;
        }

        List<CatalogueItem> items = new List<CatalogueItem>(pool.Count - 1);
        for (int index = 1; index < pool.Count; index++)
        {
            Result<WebElement> element = pool[index];
            if (element.IsFailure)
            {
                _logger.Warning(
                    "{Class} Avito catalogue item was not found",
                    nameof(ParseAvitoPages)
                );
                continue;
            }

            if (!element.Value.Attributes.ContainsKey(linkAttribute))
            {
                _logger.Warning(
                    "{Class} Avito catalogue item was not found",
                    nameof(ParseAvitoPages)
                );
                continue;
            }

            string url = element.Value.Attributes[linkAttribute];
            if (string.IsNullOrWhiteSpace(url))
            {
                _logger.Warning("{Class} Element url is empty", nameof(ParseAvitoPages));
                continue;
            }

            CatalogueItem catalogueItem = new CatalogueItem() { Url = url };

            _logger.Information(
                "{Class} Catalogue element added ({Url})",
                nameof(ParseAvitoPages),
                url
            );
            items.Add(catalogueItem);
        }

        _model.AddCatalogueItems(_model.CurrentPage, items.ToArray());
    }
}
