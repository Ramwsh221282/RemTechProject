using Rabbit.RPC.Client.Abstractions;
using RemTechAvito.Core.FiltersManagement.TransportTypes;
using RemTechAvito.Infrastructure.Contracts.Parser;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Worker.Service.Contracts.BaseImplementations;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

namespace RemTechAvito.Infrastructure.Parser;

public sealed class TransportTypesParser : BaseParser, ITransportTypesParser
{
    private const string pathType = "xpath";

    const string popularMarkButtonXpath =
        ".//button[@data-marker='popular-rubricator/controls/all']";
    const string popularMarkButton = "popular-marks-button";

    const string popularMarksRubricatorContainerXPath =
        ".//div[@class='popular-rubricator-links-o9b47']";
    const string popularMarksRubricatorName = "popular-marks-container";

    const string popularMarkRubricatorLinkXPath = ".//a[@data-marker='popular-rubricator/link']";
    const string popularMarkRubricatorName = "popular-mark-rubricator";

    public TransportTypesParser(IMessagePublisher publisher, ILogger logger)
        : base(publisher, logger) { }

    public async Task<Result<TransportTypesCollection>> Parse(CancellationToken ct = default)
    {
        WebElementPool pool = new();
        CompositeBehavior behavior = new CompositeBehavior(_logger)
            .AddBehavior(
                new CompositeBehavior()
                    .AddBehavior(new StartBehavior("none"))
                    .AddBehavior(new OpenPageBehavior(Url))
                    .AddBehavior(new ScrollToBottomBehavior())
                    .AddBehavior(new ScrollToTopBehavior())
            )
            .AddBehavior(
                new GetNewElementInstant(pool, popularMarkButtonXpath, pathType, popularMarkButton)
            )
            .AddBehavior(
                new DoForExactParent(
                    pool,
                    popularMarkButton,
                    element => new ClickOnElementRetriable(element, 50)
                )
            )
            .AddBehavior(
                new GetNewElementInstant(
                    pool,
                    popularMarksRubricatorContainerXPath,
                    pathType,
                    popularMarksRubricatorName
                )
            )
            .AddBehavior(
                new DoForExactParent(
                    pool,
                    popularMarksRubricatorName,
                    element => new GetChildrenBehavior(
                        element,
                        popularMarkRubricatorName,
                        popularMarkRubricatorLinkXPath,
                        pathType
                    )
                )
            )
            .AddBehavior(
                new DoForExactParent(
                    pool,
                    popularMarksRubricatorName,
                    element => new ParallerlTextInstantiation(element)
                )
            )
            .AddBehavior(new StopBehavior());

        using WebDriverSession session = new(_publisher);

        Result execution = await session.ExecuteBehavior(behavior, ct);
        if (execution.IsFailure)
            return execution.Error;

        Result<WebElement> element = pool.GetWebElement(el =>
            el.Name == popularMarksRubricatorName
        );
        if (element.IsFailure)
            return element.Error;

        TransportTypesCollection collection = [];
        foreach (var child in element.Value.Childs)
        {
            Result<TransportType> type = TransportType.Create(child.Text);
            if (type.IsFailure)
                return type.Error;

            Result adding = collection.Add(type);
            if (adding.IsFailure)
                return adding.Error;
        }

        pool.Clear();

        _logger.Information(
            "{Parser} parsed Transport Types {Count}",
            nameof(TransportTypesParser),
            collection.Count
        );

        return collection;
    }

    private sealed class ParallerlTextInstantiation(WebElement element) : IWebDriverBehavior
    {
        public async Task<Result> Execute(
            IMessagePublisher publisher,
            CancellationToken ct = default
        )
        {
            List<Task<Result>> tasksCollection = [];
            tasksCollection.AddRange(
                element
                    .Childs.Select(child => new InitializeTextBehavior(child))
                    .Select(textInstantiation => textInstantiation.Execute(publisher, ct))
            );

            await Task.WhenAll(tasksCollection);
            return Result.Success();
        }
    }
}
