using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Worker.Service.Contracts.BaseImplementations;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

namespace RemTechAvito.Infrastructure.Parser.CatalogueParsing.Models.CustomBehaviors.ConcreteAdvertisementParsing;

internal sealed class ParseDescriptionBehavior : IWebDriverBehavior
{
    private const string pathType = "xpath";
    private const string descriptionPath = ".//div[@itemprop='description']";
    private const string description = "description";
    private readonly CatalogueItem _item;
    private readonly ILogger _logger;

    public ParseDescriptionBehavior(CatalogueItem item, ILogger logger)
    {
        _item = item;
        _logger = logger;
    }

    public async Task<Result> Execute(IMessagePublisher publisher, CancellationToken ct = default)
    {
        WebElementPool pool = new WebElementPool();
        GetNewElementRetriable getDescription = new GetNewElementRetriable(
            pool,
            descriptionPath,
            pathType,
            description,
            5
        );
        ClearPoolBehavior clear = new ClearPoolBehavior();

        await getDescription.Execute(publisher, ct);
        await clear.Execute(publisher, ct);

        InitializeDescription(pool);
        return Result.Success();
    }

    private void InitializeDescription(WebElementPool pool)
    {
        try
        {
            Result<WebElement> element = pool[^1];
            if (element.IsFailure)
            {
                _logger.Error(
                    "{Action} description was not found",
                    nameof(ParseDescriptionBehavior)
                );
                return;
            }

            _item.Description = element.Value.Model.ElementInnerText;
            _logger.Information(
                "{Action} description {Text}",
                nameof(ParseDescriptionBehavior),
                _item.Description
            );
        }
        catch (Exception ex)
        {
            _logger.Fatal("{Action} {Exception}", nameof(ParseTitleBehavior), ex.Message);
        }
    }
}
