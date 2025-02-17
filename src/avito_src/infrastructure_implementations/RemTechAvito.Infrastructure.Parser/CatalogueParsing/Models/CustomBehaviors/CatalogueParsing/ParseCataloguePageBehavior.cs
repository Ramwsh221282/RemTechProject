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
    private const string pathType = "xpath";
    private const string cataloguePath = ".//div[@data-marker='catalog-serp']";
    private const string catalogue = "catalogue";

    public ParseCataloguePageBehavior(CataloguePageModel model, ILogger logger)
    {
        _model = model;
        _logger = logger;
    }

    public async Task<Result> Execute(IMessagePublisher publisher, CancellationToken ct = default)
    {
        WebElementPool pool = new WebElementPool();
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
        CompositeBehavior pipeline = new CompositeBehavior()
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
            Result<WebElement> container = pool[^1];
            if (container.IsFailure)
            {
                _logger.Error("Container was not found");
                return;
            }

            HtmlNode node = HtmlNode.CreateNode(container.Value.OuterHTML);
            _logger.Information("Created items node url tree");
            HtmlNodeCollection items = node.ChildNodes;
            if (items.Count == 0)
            {
                _logger.Error("Items count was 0");
                return;
            }

            foreach (var catalogueItem in items)
            {
                if (catalogueItem == null)
                    continue;

                HtmlNode? linkNode = catalogueItem.SelectSingleNode(
                    ".//div[@class='iva-item-listTopBlock-K5zdG']"
                );
                if (linkNode == null)
                    continue;

                HtmlNode? linkContainer = linkNode.SelectSingleNode(".//a[@itemprop='url']");
                if (linkContainer == null)
                    continue;

                IEnumerable<HtmlAttribute> linkAttributes = linkContainer.GetAttributes();
                HtmlAttribute? linkAttribute = linkAttributes.FirstOrDefault(a => a.Name == "href");
                if (linkAttribute == null)
                    continue;

                ReadOnlySpan<char> domain = TransportTypesParser.GetDomainUrlPart();
                ReadOnlySpan<char> url = linkAttribute.Value;

                CatalogueItem item = new CatalogueItem();
                item.Url = $"{domain}{url}";
                item.Title = linkContainer.InnerText.CleanString();
                InitializeIdIfExists(catalogueItem, item);
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
        HtmlAttribute? attribute = attributes.FirstOrDefault(a => a.Name == "data-item-id");
        if (attribute != null)
            item.Id = attribute.Value.CleanString();
    }

    private void InitializeDescriptionIfExists(HtmlNode node, CatalogueItem item)
    {
        HtmlNode? descriptionNode = node.SelectSingleNode(
            ".//div[@class='iva-item-bottomBlock-FhNhY']"
        );
        if (descriptionNode != null)
            item.Description = descriptionNode.FirstChild.InnerText.CleanString();
    }

    private void InitializePriceIfExists(HtmlNode node, CatalogueItem item)
    {
        HtmlNode? priceContainer = node.SelectSingleNode(".//p[@data-marker='item-price']");
        if (priceContainer == null)
            return;
        HtmlNodeCollection childs = priceContainer.ChildNodes;
        foreach (var child in childs)
        {
            if (child == null)
                return;

            HtmlAttribute? propertyAttribute = child.Attributes.FirstOrDefault(a =>
                a.Name == "itemprop"
            );
            if (propertyAttribute == null)
                continue;

            if (propertyAttribute.Value == "priceCurrency")
            {
                HtmlAttribute? currencyAttribute = child.Attributes.FirstOrDefault(a =>
                    a.Name == "content"
                );
                if (currencyAttribute != null)
                    item.Price.Currency = currencyAttribute.Value;
            }

            if (propertyAttribute.Value == "price")
            {
                HtmlAttribute? currencyAttribute = child.Attributes.FirstOrDefault(a =>
                    a.Name == "content"
                );
                if (currencyAttribute != null)
                    item.Price.Value = currencyAttribute.Value;
            }
        }

        HtmlNode extraPriceData = priceContainer.SelectSingleNode(
            ".//span[@class='styles-module-size_l-j3Csw styles-module-size_l_dense-JjSpL']"
        );
        if (extraPriceData == null)
            return;
        HtmlNode lastChild = extraPriceData.LastChild;
        ReadOnlySpan<char> extraTextSpan = lastChild.InnerText;
        int index = extraTextSpan.IndexOf(';');
        item.Price.Extra = $"{extraTextSpan.Slice(index + 1)}";
    }

    private void InitializeAddressIfExists(HtmlNode node, CatalogueItem item)
    {
        HtmlNode? geoNode = node.SelectSingleNode(".//div[@class='geo-root-NrkbV']");
        if (geoNode == null)
            return;
        item.Address = geoNode.InnerText;
    }
}
