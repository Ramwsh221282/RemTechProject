using HtmlAgilityPack;
using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Worker.Service.Contracts.BaseImplementations;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

namespace RemTechAvito.Infrastructure.Parser.CatalogueParsing.Models.CustomBehaviors.ConcreteAdvertisementParsing;

internal sealed class ParseTitleBehavior : IWebDriverBehavior
{
    private const string pathType = "xpath";
    private const string titlePath = ".//h1[@data-marker='item-view/title-info']";
    private const string title = "title";

    private readonly CatalogueItem _item;
    private readonly ILogger _logger;

    public ParseTitleBehavior(CatalogueItem item, ILogger logger)
    {
        _item = item;
        _logger = logger;
    }

    public async Task<Result> Execute(IMessagePublisher publisher, CancellationToken ct = default)
    {
        WebElementPool pool = new WebElementPool();
        GetNewElementRetriable getTitle = new GetNewElementRetriable(
            pool,
            titlePath,
            pathType,
            title,
            5
        );
        ClearPoolBehavior clear = new ClearPoolBehavior();

        await getTitle.Execute(publisher, ct);
        await clear.Execute(publisher, ct);

        InitializeTitle(pool);
        return Result.Success();
    }

    private void InitializeTitle(WebElementPool pool)
    {
        Result<WebElement> element = pool[^1];
        if (element.IsFailure)
        {
            _logger.Information("{Action} cannot title was not found", nameof(ParseTitleBehavior));
            return;
        }
        try
        {
            HtmlNode node = HtmlNode.CreateNode(element.Value.Model.ElementOuterHTML);
            _item.Title = node.GetDirectInnerText();
            _logger.Information("Item title: {Title}", _item.Title);
        }
        catch (Exception ex)
        {
            _logger.Fatal("{Action} {Exception}", nameof(ParseTitleBehavior), ex.Message);
        }
    }
}
