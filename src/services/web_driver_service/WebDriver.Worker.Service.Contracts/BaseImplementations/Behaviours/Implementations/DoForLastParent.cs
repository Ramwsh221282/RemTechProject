using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;

namespace WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

public sealed class DoForLastParent(
    WebElementPool pool,
    Func<WebElement, IWebDriverBehavior> factory
) : IWebDriverBehavior
{
    public async Task<Result> Execute(IMessagePublisher publisher, CancellationToken ct = default)
    {
        Result<WebElement> element = pool[pool.Count - 1];
        if (element.IsFailure)
            return new Error("No last element in pool");

        IWebDriverBehavior behavior = factory.Invoke(element);
        return await behavior.Execute(publisher, ct);
    }
}
