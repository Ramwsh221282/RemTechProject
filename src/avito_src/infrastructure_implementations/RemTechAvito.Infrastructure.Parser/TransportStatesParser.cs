using System.Runtime.CompilerServices;
using Rabbit.RPC.Client.Abstractions;
using RemTechAvito.Core.FiltersManagement.TransportStates;
using RemTechAvito.Infrastructure.Contracts.Parser.FiltersParsing;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Worker.Service.Contracts.BaseImplementations;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

namespace RemTechAvito.Infrastructure.Parser;

public sealed class TransportStatesParser(IMessagePublisher publisher, ILogger logger)
    : BaseParser(publisher, logger),
        ITransportStatesParser
{
    private const string pathType = "xpath";

    private const string stateAllXpath = ".//label[@data-marker='params[110276]/all']";
    private const string stateAll = "Все";

    private const string stateNewXpath = ".//label[@data-marker='params[110276]/426646']";
    private const string stateNew = "Новые";

    private const string stateBUXpath = ".//label[@data-marker='params[110276]/426647']";
    private const string stateBU = "Б/у";

    public async IAsyncEnumerable<Result<TransportState>> Parse(
        [EnumeratorCancellation] CancellationToken ct = default
    )
    {
        StartRetriable start = new("none", 10);
        Result starting = await start.Execute(publisher, ct);
        if (starting.IsFailure)
        {
            _logger.Error(
                "{ClassName} cannot execute. Error: {Error}",
                nameof(TransportStatesParser),
                starting.Error.Description
            );
            yield return starting.Error;
            yield break;
        }

        WebElementPool pool = new();
        OpenPageBehavior open = new(Url);
        ScrollToBottomRetriable bottom = new(10);
        ScrollToTopRetriable top = new(10);
        GetNewElementRetriable statesAllContainer =
            new(pool, stateAllXpath, pathType, stateAll, 10);
        GetNewElementRetriable stateNewContainer = new(pool, stateNewXpath, pathType, stateNew, 10);
        GetNewElementRetriable stateBuContainer = new(pool, stateBUXpath, pathType, stateBU, 10);
        StopBehavior stop = new();

        Result opening = await open.Execute(publisher, ct);
        if (opening.IsFailure)
        {
            _logger.Error(
                "{ClassName} cannot execute. Error: {Error}",
                nameof(TransportStatesParser),
                starting.Error.Description
            );
            yield return starting.Error;
            yield break;
        }

        await bottom.Execute(publisher, ct);
        await top.Execute(publisher, ct);
        await statesAllContainer.Execute(publisher, ct);
        await stateNewContainer.Execute(publisher, ct);
        await stateBuContainer.Execute(publisher, ct);
        await stop.Execute(publisher, ct);

        DateOnly date = DateOnly.FromDateTime(DateTime.Now);
        foreach (WebElement element in pool)
        {
            string name = element.InnerText;
            yield return TransportState.Create(name, date);
        }
    }
}
