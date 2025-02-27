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

    public async IAsyncEnumerable<Result<SystemTransportType>> Parse(
        [EnumeratorCancellation] CancellationToken ct = default
    )
    {
        WebElementPool pool = new();
        var start = new StartRetriable("none", 10);
        var starting = await start.Execute(_publisher, ct);
        if (starting.IsFailure)
        {
            yield return starting.Error;
            yield break;
        }

        var open = new OpenPageBehavior(Url);
        var bottom = new ScrollToBottomRetriable(10);
        var top = new ScrollToTopRetriable(10);
        var getButton = new GetNewElementRetriable(
            pool,
            popularMarkButtonXpath,
            pathType,
            popularMarkButton,
            10
        );
        var stop = new StopBehavior();

        await open.Execute(_publisher, ct);
        await bottom.Execute(_publisher, ct);
        await top.Execute(_publisher, ct);
        await getButton.Execute(_publisher, ct);

        var button = pool[^1];
        if (button.IsFailure)
        {
            yield return new Error("No rubricator button found");
            await stop.Execute(_publisher, ct);
            yield break;
        }

        var click = new ClickOnElementRetriable(button, 10);
        await click.Execute(_publisher, ct);

        var getContainer = new GetNewElementRetriable(
            pool,
            popularMarksRubricatorContainerXPath,
            pathType,
            popularMarksRubricatorName,
            10
        );

        await getContainer.Execute(_publisher, ct);
        var clear = new ClearPoolBehavior();
        await clear.Execute(_publisher, ct);

        var container = pool[^1];
        if (container.IsFailure)
        {
            yield return new Error("No container found");
            await stop.Execute(_publisher, ct);
            yield break;
        }

        var doc = new HtmlDocument();
        doc.LoadHtml(container.Value.OuterHTML);

        var nodes = doc.DocumentNode.SelectNodes(popularMarkRubricatorLinkXPath);
        var date = DateOnly.FromDateTime(DateTime.Now);

        foreach (var node in nodes)
        {
            var hrefAttribute = node.Attributes.First(a =>
                a.Name == popularMarkRubricatorAttribute
            );
            if (hrefAttribute == null)
                continue;
            var name = node.InnerText.CleanString();
            var link = PostProcessLink(hrefAttribute.Value);
            var type = SystemTransportType.Create(name, link, date);
            if (type.IsFailure)
                yield return type.Error;
            var system = type.Value.Unwrap<SystemTransportType>();
            if (system.IsFailure)
                yield return system.Error;
            yield return system;
        }

        await stop.Execute(_publisher, ct);
    }

    internal static ReadOnlySpan<char> GetDomainUrlPart()
    {
        return ['h', 't', 't', 'p', 's', ':', '/', '/', 'a', 'v', 'i', 't', 'o', '.', 'r', 'u'];
    }

    private static string PostProcessLink(ReadOnlySpan<char> href)
    {
        var index = href.IndexOf('?');
        var domain = GetDomainUrlPart();
        var processed = href.Slice(0, index);
        return $"{domain}{processed}";
    }
}
