using HtmlAgilityPack;
using Rabbit.RPC.Client.Abstractions;
using RemTechAvito.Core.FiltersManagement.TransportTypes;
using RemTechAvito.Infrastructure.Contracts.Parser;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Worker.Service.Contracts.BaseImplementations;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

namespace RemTechAvito.Infrastructure.Parser;

public sealed class TransportTypesParser : BaseParser, ITransportTypesParser
{
    private const string pathType = "xpath";

    private const string popularMarkButtonXpath =
        ".//button[@data-marker='popular-rubricator/controls/all']";
    private const string popularMarkButton = "popular-marks-button";

    private const string popularMarksRubricatorContainerXPath =
        ".//div[@class='popular-rubricator-links-o9b47']";
    private const string popularMarksRubricatorName = "popular-marks-container";

    private const string popularMarkRubricatorLinkXPath =
        ".//a[@data-marker='popular-rubricator/link']";
    private const string popularMarkRubricatorName = "popular-mark-rubricator";
    private const string popularMarkRubricatorAttribute = "href";

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
            .AddBehavior(new StopBehavior())
            .AddBehavior(new ClearPoolBehavior());

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
            string outerHTML = child.OuterHTML;
            HtmlNode node = HtmlNode.CreateNode(outerHTML);
            HtmlAttribute? hrefAttribute = node.Attributes[popularMarkRubricatorAttribute];
            if (hrefAttribute == null)
                continue;

            string name = child.OuterHTML;
            string href = PostProcessLink(hrefAttribute.Value);

            Result<TransportType> type = TransportType.Create(name, href);
            if (type.IsFailure)
                continue;

            collection.Add(type);
        }

        pool.Clear();

        _logger.Information(
            "{Parser} parsed Transport Types {Count}",
            nameof(TransportTypesParser),
            collection.Count
        );

        return collection;
    }

    public static ReadOnlySpan<char> GetDomainUrlPart() =>
        ['h', 't', 't', 'p', 's', ':', '/', '/', 'a', 'v', 'i', 't', 'o', '.', 'r', 'u'];

    private static string PostProcessLink(ReadOnlySpan<char> href)
    {
        int index = href.IndexOf('?');
        ReadOnlySpan<char> domain = GetDomainUrlPart();
        ReadOnlySpan<char> processed = href.Slice(0, index);
        return $"{domain}{processed}";
    }
}
