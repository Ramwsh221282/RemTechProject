using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Worker.Service.Contracts.BaseImplementations;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

namespace RemTechAvito.Infrastructure.Parser.CatalogueParsing.Models.CustomBehaviors;

internal sealed class ParseTitleBehavior : IWebDriverBehavior
{
    private const string pathType = "xpath";
    private const string titlePath = ".//h1[@data-marker='item-view/title-info']";
    private const string title = "title";

    private readonly CatalogueItem _item;

    public ParseTitleBehavior(CatalogueItem item) => _item = item;

    public async Task<Result> Execute(IMessagePublisher publisher, CancellationToken ct = default)
    {
        WebElementPool pool = new WebElementPool();
        CompositeBehavior pipeLine = new CompositeBehavior()
            .AddBehavior(new GetNewElementRetriable(pool, titlePath, pathType, title, 10))
            .AddBehavior(new DoForAllParents(pool, element => new InitializeTextBehavior(element)));

        await pipeLine.Execute(publisher, ct);
        Result<WebElement> titleElement = pool[0];

        if (titleElement.IsFailure)
            return titleElement;

        _item.Title = titleElement.Value.Text;

        return Result.Success();
    }
}
