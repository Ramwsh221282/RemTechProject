using Rabbit.RPC.Client.Abstractions;
using RemTechAvito.Core.FiltersManagement.CustomerTypes;
using RemTechAvito.Infrastructure.Contracts.Parser;
using RemTechAvito.Infrastructure.Contracts.Parser.FiltersParsing;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Worker.Service.Contracts.BaseImplementations;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

namespace RemTechAvito.Infrastructure.Parser;

public sealed class CustomerTypesParser(IMessagePublisher publisher, ILogger logger)
    : BaseParser(publisher, logger),
        ICustomerTypesParser
{
    private const string pathType = "xpath";

    private const string containerPath = ".//div[@data-marker='user' and @role='group']";
    private const string containerName = "customers-type";

    private const string radioButtonPath = ".//label";
    private const string radioButton = "radio-button";

    public async Task<Result<CustomerTypesCollection>> Parse(CancellationToken ct = default)
    {
        WebElementPool pool = new();
        CompositeBehavior pipeline = new CompositeBehavior(_logger)
            .AddBehavior(
                new StartBehavior("none"),
                new OpenPageBehavior(Url).WithWait(10),
                new ScrollToBottomBehavior(),
                new ScrollToTopBehavior()
            )
            .AddBehavior(new GetNewElementInstant(pool, containerPath, pathType, containerName))
            .AddBehavior(new ScrollToBottomBehavior())
            .AddBehavior(
                new DoForExactParent(
                    pool,
                    containerName,
                    element => new GetChildrenBehavior(
                        element,
                        radioButton,
                        radioButtonPath,
                        pathType
                    )
                )
            )
            .AddBehavior(new StopBehavior())
            .AddBehavior(new ClearPoolBehavior());

        using WebDriverSession session = new(_publisher);
        Result execution = await session.ExecuteBehavior(pipeline, ct);
        if (execution.IsFailure)
            return execution.Error;

        Result<WebElement> container = pool.GetWebElement(el => el.Name == containerName);
        if (container.IsFailure)
            return container.Error;

        CustomerTypesCollection collection = [];
        foreach (var child in container.Value.Childs)
        {
            Result<CustomerType> type = CustomerType.Create(child.InnerText);
            if (type.IsFailure)
                return type.Error;
            collection.Add(type);
        }

        pool.Clear();

        _logger.Information(
            "{Parser} parsed Customer Types {Count}",
            nameof(TransportTypesParser),
            collection.Count
        );

        return collection;
    }
}
