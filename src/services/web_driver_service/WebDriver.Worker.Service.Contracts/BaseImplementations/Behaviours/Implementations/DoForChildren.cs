using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;

namespace WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

public sealed class DoForChildren(
    WebElementPool pool,
    string parentName,
    params Func<WebElement, IWebDriverBehavior>[] behaviorFactories
) : IWebDriverBehavior
{
    private int _limit;
    private int _tries;

    public async Task<Result> Execute(IMessagePublisher publisher, CancellationToken ct = default)
    {
        Result<WebElement> root = pool.GetWebElement(el => el.Name == parentName);
        if (root.IsFailure)
            return root.Error;

        foreach (var factory in behaviorFactories)
        {
            if (_tries == _limit - 1)
                break;
            await root.Value.ExecuteForChilds(publisher, factory, ct);
            _tries++;
        }

        return Result.Success();
    }

    public DoForChildren WithLimit(int limit)
    {
        _limit = limit;
        return this;
    }
}
