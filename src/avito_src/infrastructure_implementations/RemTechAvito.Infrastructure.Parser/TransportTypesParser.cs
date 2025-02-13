using Rabbit.RPC.Client.Abstractions;
using RemTechAvito.Core.FiltersManagement.TransportTypes;
using RemTechAvito.Infrastructure.Contracts.Parser;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Worker.Service.Contracts.BaseImplementations;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

namespace RemTechAvito.Infrastructure.Parser;

public sealed class TransportTypesParser(IMessagePublisher publisher) : ITransportTypesParser
{
    private const string Url =
        "https://www.avito.ru/all/gruzoviki_i_spetstehnika/pogruzchiki-ASgBAgICAURU4E0";
    private const string pathType = "xpath";

    private const string filterInputPath =
        ".//input[@data-marker='params[111024]/multiselect-outline-input/input']";
    private const string filterInput = "filter-input";

    private const string checkBoxContainerPath = ".//div[@data-marker='params[111024]/list']";
    private const string checkBoxesContainer = "check-boxes-container";

    private const string checkBoxPath = ".//label[@role='checkbox']";
    private const string checkBox = "check-box";

    public async Task<Result<TransportTypesCollection>> Parse(CancellationToken ct = default)
    {
        WebElementPool pool = new();
        CompositeBehavior behavior = new CompositeBehavior()
            .AddBehavior(new StartBehavior("none"))
            .AddBehavior(new OpenPageBehavior(Url).WithWait(5))
            .AddBehavior(new ScrollToBottomBehavior())
            .AddBehavior(new ScrollToTopBehavior())
            .AddBehavior(new GetSingleElementBehavior(pool, filterInputPath, pathType, filterInput))
            .AddBehavior(
                new DoForExactParent(
                    pool,
                    filterInput,
                    element => new ClickOnElementInstant(element)
                )
            )
            .AddBehavior(
                new GetSingleElementBehavior(
                    pool,
                    checkBoxContainerPath,
                    pathType,
                    checkBoxesContainer
                )
            )
            .AddBehavior(
                new DoForExactParent(
                    pool,
                    checkBoxesContainer,
                    [element => new GetChildrenBehavior(element, checkBox, checkBoxPath, pathType)]
                )
            )
            .AddBehavior(
                new DoForAllChildren(
                    pool,
                    parentName: checkBoxesContainer,
                    factories: element => new InitializeTextBehavior(element)
                )
            )
            .AddBehavior(new StopBehavior());

        using WebDriverSession session = new(publisher);

        Result execution = await session.ExecuteBehavior(behavior, ct);
        if (execution.IsFailure)
            return execution.Error;

        Result<WebElement> element = pool.GetWebElement(el => el.Name == checkBoxesContainer);
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
        return collection;
    }
}
