using HtmlAgilityPack;
using Rabbit.RPC.Client.Abstractions;
using RemTechAvito.Infrastructure.Parser.Extensions;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Worker.Service.Contracts.BaseImplementations;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

namespace RemTechAvito.Infrastructure.Parser.CatalogueParsing.Models.CustomBehaviors.ConcreteAdvertisementParsing;

internal sealed class ParseDateBehavior : IWebDriverBehavior
{
    private const string pathType = "xpath";
    private const string footerPath = ".//div[@class='style-item-footer-Ufxh_']";
    private const string footer = "footer";
    private readonly CatalogueItem _item;
    private readonly ILogger _logger;

    public ParseDateBehavior(CatalogueItem item, ILogger logger)
    {
        _item = item;
        _logger = logger;
    }

    public async Task<Result> Execute(IMessagePublisher publisher, CancellationToken ct = default)
    {
        WebElementPool pool = new WebElementPool();
        GetNewElementRetriable getFooter = new GetNewElementRetriable(
            pool,
            footerPath,
            pathType,
            footer,
            10
        );
        ClearPoolBehavior clear = new ClearPoolBehavior();

        await getFooter.Execute(publisher, ct);
        await clear.Execute(publisher, ct);

        InitializeDateAsString(pool);
        return Result.Success();
    }

    private void InitializeDateAsString(WebElementPool pool)
    {
        try
        {
            Result<WebElement> element = pool[^1];
            if (element.IsFailure)
            {
                _logger.Error("{Action} Date is not found", nameof(ParseDateBehavior));
                return;
            }

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(element.Value.OuterHTML);
            HtmlNode articleNode = doc.DocumentNode.FirstChild;
            HtmlNode container = articleNode.FirstChild;
            HtmlNode? dateNode = container.SelectSingleNode(
                ".//span[@data-marker='item-view/item-date']"
            );
            if (dateNode == null)
                return;

            HtmlNode dateText = dateNode.LastChild;

            _item.Date = dateText.InnerText.CleanString();
            _logger.Information("{Action} Date {Text}", nameof(ParseDateBehavior), _item.Date);
        }
        catch (Exception ex)
        {
            _logger.Fatal("{Action} {Exception}", nameof(ParseDateBehavior), ex.Message);
        }
    }
}
