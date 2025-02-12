using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;

namespace WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

public sealed class DoForParent(
    WebElementPool pool,
    string parentName,
    params Func<WebElement, IWebDriverBehavior>[] behaviorFactories
) : IWebDriverBehavior
{
    public async Task<Result> Execute(IMessagePublisher publisher, CancellationToken ct = default)
    {
        Result<WebElement> element = pool.GetWebElement(el => el.Name == parentName);
        if (element.IsFailure)
            return element.Error;

        foreach (var factory in behaviorFactories)
        {
            IWebDriverBehavior behavior = factory.Invoke(element.Value);
            await behavior.Execute(publisher, ct);
        }

        return Result.Success();
    }
}
