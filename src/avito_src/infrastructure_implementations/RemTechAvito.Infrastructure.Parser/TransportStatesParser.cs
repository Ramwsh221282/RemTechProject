using Rabbit.RPC.Client.Abstractions;
using RemTechAvito.Core.FiltersManagement.TransportStates;
using RemTechAvito.Infrastructure.Contracts.Parser;
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

    public async Task<Result<TransportStatesCollection>> Parse(CancellationToken ct = default)
    {
        WebElementPool pool = new WebElementPool();
        CompositeBehavior behavior = new CompositeBehavior(_logger)
            .AddBehavior(
                new StartBehavior("none"),
                new OpenPageBehavior(Url),
                new ScrollToBottomBehavior(),
                new ScrollToTopBehavior()
            )
            .AddBehavior(new GetNewElementInstant(pool, stateAllXpath, pathType, stateAll))
            .AddBehavior(new GetNewElementInstant(pool, stateNewXpath, pathType, stateNew))
            .AddBehavior(new GetNewElementInstant(pool, stateBUXpath, pathType, stateBU))
            .AddBehavior(new StopBehavior())
            .AddBehavior(new ClearPoolBehavior());

        using WebDriverSession session = new WebDriverSession(_publisher);
        Result result = await session.ExecuteBehavior(behavior, ct);

        if (result.IsFailure)
            return result.Error;

        TransportStatesCollection collection = [];
        foreach (var element in pool.Elements)
        {
            Result<TransportState> state = TransportState.Create(element.Model.ElementInnerText);
            if (state.IsFailure)
                return state.Error;

            collection.Add(state);
        }

        pool.Clear();

        _logger.Information(
            "{Parser} parsed Transport States {Count}",
            nameof(TransportTypesParser),
            collection.Count
        );

        return collection;
    }
}
