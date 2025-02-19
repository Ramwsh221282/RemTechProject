using System.Runtime.CompilerServices;
using HtmlAgilityPack;
using Rabbit.RPC.Client.Abstractions;
using RemTechAvito.Core.FiltersManagement.CustomerTypes;
using RemTechAvito.Infrastructure.Contracts.Parser.FiltersParsing;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Worker.Service.Contracts.BaseImplementations;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

namespace RemTechAvito.Infrastructure.Parser;

internal sealed class CustomerTypesParser : BaseParser, ICustomerTypesParser
{
    private const string pathType = "xpath";
    private const string containerPath = ".//div[@data-marker='user' and @role='group']";
    private const string containerName = "customers-type";
    private const string radioButtonPath = ".//label";

    public CustomerTypesParser(IMessagePublisher publisher, ILogger logger)
        : base(publisher, logger) { }

    public async IAsyncEnumerable<Result<CustomerType>> Parse(
        [EnumeratorCancellation] CancellationToken ct = default
    )
    {
        StartRetriable start = new StartRetriable("none", 10);
        Result starting = await start.Execute(_publisher, ct);
        if (starting.IsFailure)
        {
            _logger.Information(
                "{Parser} cannot start. {Error}",
                nameof(CustomerTypesParser),
                starting.Error.Description
            );
            yield return starting.Error;
            yield break;
        }

        WebElementPool pool = new();
        OpenPageBehavior open = new(Url);
        ScrollToBottomRetriable bottom = new(10);
        ScrollToTopRetriable top = new(10);
        GetNewElementRetriable getContainer = new(pool, containerPath, pathType, containerName, 10);
        StopBehavior stop = new();

        Result opening = await open.Execute(_publisher, ct);
        if (opening.IsFailure)
        {
            _logger.Information(
                "{Parser} cannot open. {Error}",
                nameof(CustomerTypesParser),
                starting.Error.Description
            );
            yield return opening.Error;
            yield break;
        }

        await bottom.Execute(_publisher, ct);
        await top.Execute(_publisher, ct);
        await getContainer.Execute(_publisher, ct);
        await stop.Execute(_publisher, ct);

        Result<WebElement> container = pool[^1];
        if (container.IsFailure)
        {
            _logger.Information(
                "{Parser} cannot get customer types container. {Error}",
                nameof(CustomerTypesParser),
                starting.Error.Description
            );
            yield return container.Error;
            yield break;
        }

        HtmlDocument document = new HtmlDocument();
        document.LoadHtml(container.Value.OuterHTML);
        HtmlNodeCollection? nodes = document.DocumentNode.SelectNodes(radioButtonPath);
        if (nodes == null)
        {
            _logger.Information(
                "{Parser} cannot get customer types null. {Error}",
                nameof(CustomerTypesParser),
                starting.Error.Description
            );
            yield return container.Error;
            yield break;
        }

        DateOnly date = DateOnly.FromDateTime(DateTime.Now);
        foreach (var node in nodes)
        {
            yield return CustomerType.Create(node.InnerText, date);
        }
    }
}
