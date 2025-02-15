using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Worker.Service.Contracts.BaseImplementations;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

namespace RemTechAvito.Infrastructure.Parser.CatalogueParsing.Models.CustomBehaviors.ConcreteAdvertisementParsing;

internal sealed class ParseDateBehavior : IWebDriverBehavior
{
    private const string pathType = "xpath";
    private const string datePath = ".//span[@data-marker='item-view/item-date']";
    private const string date = "date";
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
        GetNewElementRetriable getDate = new GetNewElementRetriable(
            pool,
            datePath,
            pathType,
            date,
            5
        );
        ClearPoolBehavior clear = new ClearPoolBehavior();

        await getDate.Execute(publisher, ct);
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
                _logger.Error("{Action} date is not found", nameof(ParseDateBehavior));
                return;
            }

            _item.Date = element.Value.Model.ElementInnerText;
            _logger.Information("{Action} date {Text}", nameof(ParseDateBehavior), _item.Date);
        }
        catch (Exception ex)
        {
            _logger.Fatal("{Action} {Exception}", nameof(ParseTitleBehavior), ex.Message);
        }
    }
}
