using HtmlAgilityPack;
using Rabbit.RPC.Client.Abstractions;
using RemTechAvito.Infrastructure.Parser.Extensions;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Worker.Service.Contracts.BaseImplementations;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

namespace RemTechAvito.Infrastructure.Parser.CatalogueParsing.Models.CustomBehaviors.CatalogueParsing;

internal sealed class ParseCataloguePageBehavior : IWebDriverBehavior
{
    private readonly CataloguePageModel _model;
    private readonly ILogger _logger;
    private readonly IEnumerable<long> _existingIds;
    private const string pathType = "xpath";
    private const string cataloguePath = ".//div[@data-marker='catalog-serp']";
    private const string catalogue = "catalogue";

    public ParseCataloguePageBehavior(
        CataloguePageModel model,
        ILogger logger,
        IEnumerable<long>? existingIds = null
    )
    {
        _model = model;
        _logger = logger;
        _existingIds = existingIds ?? [];
    }

    public async Task<Result> Execute(IMessagePublisher publisher, CancellationToken ct = default)
    {
        var pool = new WebElementPool();
        await AddContainerInPool(pool, publisher, ct);
        AddItemsInModel(pool);
        return Result.Success();
    }

    private static async Task AddContainerInPool(
        WebElementPool pool,
        IMessagePublisher publisher,
        CancellationToken ct = default
    )
    {
        var pipeline = new CompositeBehavior()
            .AddBehavior(new ScrollToBottomRetriable(10))
            .AddBehavior(new ScrollToTopRetriable(10))
            .AddBehavior(new GetNewElementRetriable(pool, cataloguePath, pathType, catalogue, 10))
            .AddBehavior(new ClearPoolBehavior());

        await pipeline.Execute(publisher, ct);
    }

    private void AddItemsInModel(WebElementPool pool)
    {
        try
        {
            var container = pool[^1];
            if (container.IsFailure)
            {
                _logger.Error("Container was not found");
                return;
            }

            var node = HtmlNode.CreateNode(container.Value.OuterHTML);
            _logger.Information("Created items node url tree");
            var items = node.ChildNodes;
            if (items.Count == 0)
            {
                _logger.Error("Items count was 0");
                return;
            }

            foreach (var catalogueItem in items)
            {
                if (catalogueItem == null)
                    continue;

                var linkNode = catalogueItem.SelectSingleNode(
                    ".//div[@class='iva-item-listTopBlock-K5zdG']"
                );
                if (linkNode == null)
                    continue;

                var linkContainer = linkNode.SelectSingleNode(".//a[@itemprop='url']");
                if (linkContainer == null)
                    continue;

                IEnumerable<HtmlAttribute> linkAttributes = linkContainer.GetAttributes();
                var linkAttribute = linkAttributes.FirstOrDefault(a => a.Name == "href");
                if (linkAttribute == null)
                    continue;

                var domain = TransportTypesParser.GetDomainUrlPart();
                ReadOnlySpan<char> url = linkAttribute.Value;

                var item = new CatalogueItem();
                item.Url = $"{domain}{url}";
                item.Title = linkContainer.InnerText.CleanString();
                InitializeIdIfExists(catalogueItem, item);

                if (_existingIds.Any(i => i.ToString() == item.Id))
                    continue;

                InitializeDescriptionIfExists(catalogueItem, item);
                InitializePriceIfExists(catalogueItem, item);
                InitializeAddressIfExists(catalogueItem, item);

                _model.AddItem(item);
                _logger.Information(
                    "{Action}. Added item. Id: {Id} Url: {Url}.",
                    nameof(ParseCataloguePageBehavior),
                    item.Id,
                    item.Url
                );
            }
        }
        catch
        {
            // ignored
        }
    }

    private void InitializeIdIfExists(HtmlNode node, CatalogueItem item)
    {
        IEnumerable<HtmlAttribute> attributes = node.GetAttributes();
        var attribute = attributes.FirstOrDefault(a => a.Name == "data-item-id");
        if (attribute != null)
            item.Id = attribute.Value.CleanString();
    }

    private void InitializeDescriptionIfExists(HtmlNode node, CatalogueItem item)
    {
        var descriptionNode = node.SelectSingleNode(".//div[@class='iva-item-bottomBlock-FhNhY']");
        if (descriptionNode != null)
            item.Description = descriptionNode.FirstChild.InnerText.CleanString();
    }

    private void InitializePriceIfExists(HtmlNode node, CatalogueItem item)
    {
        var priceContainer = node.SelectSingleNode(".//p[@data-marker='item-price']");
        if (priceContainer == null)
            return;
        var childs = priceContainer.ChildNodes;
        foreach (var child in childs)
        {
            if (child == null)
                return;

            var propertyAttribute = child.Attributes.FirstOrDefault(a => a.Name == "itemprop");
            if (propertyAttribute == null)
                continue;

            if (propertyAttribute.Value == "priceCurrency")
            {
                var currencyAttribute = child.Attributes.FirstOrDefault(a => a.Name == "content");
                if (currencyAttribute != null)
                    item.Price.Currency = currencyAttribute.Value;
            }

            if (propertyAttribute.Value == "price")
            {
                var currencyAttribute = child.Attributes.FirstOrDefault(a => a.Name == "content");
                if (currencyAttribute != null)
                    item.Price.Value = currencyAttribute.Value;
            }
        }

        var extraPriceData = priceContainer.SelectSingleNode(
            ".//span[@class='styles-module-size_l-j3Csw styles-module-size_l_dense-JjSpL']"
        );
        if (extraPriceData == null)
            return;
        var lastChild = extraPriceData.LastChild;
        ReadOnlySpan<char> extraTextSpan = lastChild.InnerText;
        var index = extraTextSpan.IndexOf(';');
        item.Price.Extra = $"{extraTextSpan.Slice(index + 1)}";
    }

    private void InitializeAddressIfExists(HtmlNode node, CatalogueItem item)
    {
        var geoNode = node.SelectSingleNode(".//div[@class='geo-root-NrkbV']");
        if (geoNode == null)
            return;
        item.Address = geoNode.InnerText;
    }
}
