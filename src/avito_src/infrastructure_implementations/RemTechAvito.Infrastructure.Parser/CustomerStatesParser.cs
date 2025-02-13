using Rabbit.RPC.Client.Abstractions;
using RemTechAvito.Core.FiltersManagement.CustomerStates;
using RemTechAvito.Infrastructure.Contracts.Parser;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Worker.Service.Contracts.BaseImplementations;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

namespace RemTechAvito.Infrastructure.Parser;

public sealed class CustomerStatesParser : BaseParser, ICustomerStatesParser
{
    private const string pathType = "xpath";

    private const string ratingFourStarsAndMorePath =
        ".//label[@data-marker='params[115385]/checkbox/1212562']";
    private const string ratingFourStarsAndMore = "four-stars";

    private const string ratingCompaniesPath = ".//label[@data-marker='params[172422]/checkbox/1']";
    private const string ratingCompanies = "companies";

    public CustomerStatesParser(IMessagePublisher publisher, ILogger logger)
        : base(publisher, logger) { }

    public async Task<Result<CustomerStatesCollection>> Parse(CancellationToken ct = default)
    {
        WebElementPool pool = new();
        CompositeBehavior pipeline = new CompositeBehavior(_logger)
            .AddBehavior(
                new StartBehavior("none"),
                new OpenPageBehavior(Url).WithWait(10),
                new ScrollToBottomBehavior(),
                new ScrollToTopBehavior()
            )
            .AddBehavior(
                new GetSingleElementBehavior(
                    pool,
                    ratingFourStarsAndMorePath,
                    pathType,
                    ratingFourStarsAndMore
                )
            )
            .AddBehavior(
                new GetSingleElementBehavior(pool, ratingCompaniesPath, pathType, ratingCompanies)
            )
            .AddBehavior(new DoForAllParents(pool, element => new InitializeTextBehavior(element)))
            .AddBehavior(new StopBehavior());

        using WebDriverSession session = new(_publisher);
        Result execution = await session.ExecuteBehavior(pipeline, ct);

        if (execution.IsFailure)
            return execution.Error;

        CustomerStatesCollection collection = [];
        foreach (var element in pool.Elements)
        {
            Result<CustomerState> state = CustomerState.Create(element.Text);
            if (state.IsFailure)
                return state.Error;

            collection.Add(state);
        }

        pool.Clear();

        _logger.Information(
            "{Parser} parsed Customer States {Count}",
            nameof(TransportTypesParser),
            collection.Count
        );

        return collection;
    }
}
