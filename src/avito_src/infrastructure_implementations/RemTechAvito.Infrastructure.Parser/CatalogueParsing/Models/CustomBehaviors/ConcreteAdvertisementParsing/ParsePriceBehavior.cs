using HtmlAgilityPack;
using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Worker.Service.Contracts.BaseImplementations;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

namespace RemTechAvito.Infrastructure.Parser.CatalogueParsing.Models.CustomBehaviors.ConcreteAdvertisementParsing;

internal sealed class ParsePriceBehavior : IWebDriverBehavior
{
    private readonly CatalogueItem _item;
    private readonly ILogger _logger;

    private const string pathType = "xpath";
    private const string priceNodesContainerPath =
        ".//div[@data-marker='item-view/item-price-container']";
    private const string name = "price-container";

    private const string priceValueNodePath = ".//span[@data-marker='item-view/item-price']";
    private const string currencyNodePath = ".//span[@itemprop='priceCurrency']";
    private const string extrasNodePath = ".//span[@class='style-price-value-additional-pFInr']";
    private const string attribute = "content";

    public ParsePriceBehavior(CatalogueItem item, ILogger logger)
    {
        _item = item;
        _logger = logger;
    }

    public async Task<Result> Execute(IMessagePublisher publisher, CancellationToken ct = default)
    {
        WebElementPool pool = new WebElementPool();
        GetNewElementRetriable getPrice = new GetNewElementRetriable(
            pool,
            priceNodesContainerPath,
            pathType,
            name,
            5
        );
        ClearPoolBehavior clear = new ClearPoolBehavior();

        await getPrice.Execute(publisher, ct);
        await clear.Execute(publisher, ct);

        InitializePrice(pool);
        return Result.Success();
    }

    private void InitializePrice(WebElementPool pool)
    {
        try
        {
            Result<WebElement> element = pool[0];
            if (element.IsFailure)
            {
                _logger.Error("Cannot get price attribute of advertisement");
                return;
            }
            HtmlNode parent = HtmlNode.CreateNode(element.Value.Model.ElementOuterHTML);

            // extracting price value
            HtmlNode? priceValueNode = parent.SelectSingleNode(priceValueNodePath);
            if (priceValueNode == null)
            {
                _logger.Error("{Action} price node is null", nameof(ParsePriceBehavior));
                return;
            }
            IEnumerable<HtmlAttribute> priceValueAttributes = priceValueNode.GetAttributes();
            HtmlAttribute? priceValueAttribute = priceValueAttributes.FirstOrDefault(a =>
                a.Name == attribute
            );
            if (priceValueAttribute == null)
            {
                _logger.Error("{Action} price attribute is null", nameof(ParsePriceBehavior));
                return;
            }
            string price = priceValueAttribute.Value;

            // extracting currency value
            HtmlNode? currencyNode = parent.SelectSingleNode(currencyNodePath);
            if (currencyNode == null)
            {
                _logger.Error("{Action} currency node is null", nameof(ParsePriceBehavior));
                return;
            }
            IEnumerable<HtmlAttribute> currencyAttributes = currencyNode.GetAttributes();
            HtmlAttribute? currencyAttribute = currencyAttributes.FirstOrDefault(a =>
                a.Name == attribute
            );
            if (currencyAttribute == null)
            {
                _logger.Error("{Action} currency attribute is null", nameof(ParsePriceBehavior));
                return;
            }
            string currency = currencyAttribute.Value;

            // extracting extras value
            HtmlNode? extraInfoNodeContainer = parent.SelectSingleNode(extrasNodePath);
            if (extraInfoNodeContainer == null)
            {
                _logger.Error("{Action} extras node is null", nameof(ParsePriceBehavior));
                return;
            }
            ReadOnlySpan<char> extraSpan = extraInfoNodeContainer.InnerText;
            int index = extraSpan.IndexOf(';');
            extraSpan = extraSpan.Slice(index + 1);

            _item.Price.Value = price;
            _item.Price.Currency = currency;
            _item.Price.Extra = $"{extraSpan}";
            _logger.Information(
                "{Action}. Price: {Price}. Currency: {Currency}. Extra: {Extra}",
                nameof(ParsePriceBehavior),
                _item.Price.Value,
                _item.Price.Currency,
                _item.Price.Extra
            );
        }
        catch (Exception ex)
        {
            _logger.Fatal("{Action} {Exception}", nameof(ParsePriceBehavior), ex.Message);
        }
    }
}
