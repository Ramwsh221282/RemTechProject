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

                IEnumerable<HtmlAttribute> itemAttributes = catalogueItem.GetAttributes();
                HtmlAttribute? idAttributeItem = itemAttributes.FirstOrDefault(a =>
                    a.Name == "data-item-id"
                );

                HtmlNode? descriptionNode = catalogueItem.SelectSingleNode(
                    ".//div[@class='iva-item-bottomBlock-FhNhY']"
                );

                ReadOnlySpan<char> domain = TransportTypesParser.GetDomainUrlPart();
                ReadOnlySpan<char> url = linkAttribute.Value;
                CatalogueItem result = new CatalogueItem()
                {
                    Url = $"{domain}{url}",
                    Id = idAttributeItem == null ? "" : idAttributeItem.Value.CleanString(),
                    Description =
                        descriptionNode == null
                            ? ""
                            : descriptionNode.FirstChild.InnerText.CleanString(),
                    Title = linkContainer.InnerText.CleanString(),
                };

                _model.AddItem(result);
                _logger.Information(
                    "{Action}. Added item. Id: {Id} Url: {Url}.",
                    nameof(ParseCataloguePageBehavior),
                    result.Id,
                    result.Url
                );
            }
        }
        catch
        {
            // ignored
        }
    }
}
