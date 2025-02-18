using System.Runtime.CompilerServices;
using Rabbit.RPC.Client.Abstractions;
using RemTechAvito.Core.FiltersManagement.CustomerStates;
using RemTechAvito.Infrastructure.Contracts.Parser.FiltersParsing;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Worker.Service.Contracts.BaseImplementations;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

namespace RemTechAvito.Infrastructure.Parser;

internal sealed class CustomerStatesParser : BaseParser, ICustomerStatesParser
{
    private const string pathType = "xpath";

    private const string ratingFourStarsAndMorePath =
        ".//label[@data-marker='params[115385]/checkbox/1212562']";
    private const string ratingFourStarsAndMore = "four-stars";

    private const string ratingCompaniesPath = ".//label[@data-marker='params[172422]/checkbox/1']";
    private const string ratingCompanies = "companies";

    public CustomerStatesParser(IMessagePublisher publisher, ILogger logger)
        : base(publisher, logger) { }

    public async IAsyncEnumerable<Result<CustomerState>> Parse(
        [EnumeratorCancellation] CancellationToken ct = default
    )
    {
        StartRetriable start = new StartRetriable("none", 10);
        Result starting = await start.Execute(_publisher, ct);
        if (starting.IsFailure)
        {
            _logger.Error(
                "{Parser} cannot start. Error: {error}",
                nameof(CustomerStatesParser),
                starting.Error.Description
            );
            yield return starting.Error;
            yield break;
        }

        WebElementPool pool = new();
        OpenPageBehavior open = new(Url);
        ScrollToBottomRetriable bottom = new(10);
        ScrollToTopRetriable top = new(10);
        StopBehavior stop = new StopBehavior();
        GetNewElementRetriable getFourStars =
            new(pool, ratingFourStarsAndMorePath, pathType, ratingFourStarsAndMore, 10);
        GetNewElementRetriable getCompanies =
            new(pool, ratingCompaniesPath, pathType, ratingCompanies, 10);

        Result opening = await open.Execute(_publisher, ct);
        if (opening.IsFailure)
        {
            _logger.Error(
                "{Parser} cannot open url. Error: {error}",
                nameof(CustomerStatesParser),
                opening.Error.Description
            );
            yield return opening.Error;
            yield break;
        }

        await bottom.Execute(_publisher, ct);
        await top.Execute(_publisher, ct);
        await getFourStars.Execute(_publisher, ct);
        await getCompanies.Execute(_publisher, ct);
        await stop.Execute(_publisher, ct);

        DateOnly date = DateOnly.FromDateTime(DateTime.Now);
        foreach (var element in pool)
        {
            yield return CustomerState.Create(element.InnerText, date);
        }
    }
}
