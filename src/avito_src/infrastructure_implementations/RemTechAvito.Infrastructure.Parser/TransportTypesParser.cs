using System.Runtime.CompilerServices;
using HtmlAgilityPack;
using Rabbit.RPC.Client.Abstractions;
using RemTechAvito.Core.FiltersManagement.TransportTypes;
using RemTechAvito.Infrastructure.Contracts.Parser.FiltersParsing;
using RemTechAvito.Infrastructure.Parser.Extensions;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Worker.Service.Contracts.BaseImplementations;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

namespace RemTechAvito.Infrastructure.Parser;

internal sealed class TransportTypesParser : BaseParser, ITransportTypesParser
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
    private const string popularMarkRubricatorAttribute = "href";

    public TransportTypesParser(IMessagePublisher publisher, ILogger logger)
        : base(publisher, logger) { }

    public async IAsyncEnumerable<Result<TransportType>> Parse(
        [EnumeratorCancellation] CancellationToken ct = default
    )
    {
        WebElementPool pool = new();
        StartRetriable start = new StartRetriable("none", 10);
        Result starting = await start.Execute(_publisher, ct);
        if (starting.IsFailure)
        {
            yield return starting.Error;
            yield break;
        }

        OpenPageBehavior open = new OpenPageBehavior(Url);
        ScrollToBottomRetriable bottom = new ScrollToBottomRetriable(10);
        ScrollToTopRetriable top = new ScrollToTopRetriable(10);
        GetNewElementRetriable getButton = new GetNewElementRetriable(
            pool,
            popularMarkButtonXpath,
            pathType,
            popularMarkButton,
            10
        );
        StopBehavior stop = new StopBehavior();

        await open.Execute(_publisher, ct);
        await bottom.Execute(_publisher, ct);
        await top.Execute(_publisher, ct);
        await getButton.Execute(_publisher, ct);

        Result<WebElement> button = pool[^1];
        if (button.IsFailure)
        {
            yield return new Error("No rubricator button found");
            await stop.Execute(_publisher, ct);
            yield break;
        }

        ClickOnElementRetriable click = new ClickOnElementRetriable(button, 10);
        await click.Execute(_publisher, ct);

        GetNewElementRetriable getContainer = new GetNewElementRetriable(
            pool,
            popularMarksRubricatorContainerXPath,
            pathType,
            popularMarksRubricatorName,
            10
        );

        await getContainer.Execute(_publisher, ct);
        ClearPoolBehavior clear = new ClearPoolBehavior();
        await clear.Execute(_publisher, ct);

        Result<WebElement> container = pool[^1];
        if (container.IsFailure)
        {
            yield return new Error("No container found");
            await stop.Execute(_publisher, ct);
            yield break;
        }

        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(container.Value.OuterHTML);

        HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes(popularMarkRubricatorLinkXPath);
        DateOnly date = DateOnly.FromDateTime(DateTime.Now);

        foreach (var node in nodes)
        {
            HtmlAttribute? hrefAttribute = node.Attributes.First(a =>
                a.Name == popularMarkRubricatorAttribute
            );
            if (hrefAttribute == null)
                continue;
            string name = node.InnerText.CleanString();
            string link = PostProcessLink(hrefAttribute.Value);
            Result<TransportType> type = TransportType.Create(name, link, date);
            yield return type;
        }
        await stop.Execute(_publisher, ct);
    }

    internal static ReadOnlySpan<char> GetDomainUrlPart() =>
        ['h', 't', 't', 'p', 's', ':', '/', '/', 'a', 'v', 'i', 't', 'o', '.', 'r', 'u'];

    private static string PostProcessLink(ReadOnlySpan<char> href)
    {
        int index = href.IndexOf('?');
        ReadOnlySpan<char> domain = GetDomainUrlPart();
        ReadOnlySpan<char> processed = href.Slice(0, index);
        return $"{domain}{processed}";
    }
}
