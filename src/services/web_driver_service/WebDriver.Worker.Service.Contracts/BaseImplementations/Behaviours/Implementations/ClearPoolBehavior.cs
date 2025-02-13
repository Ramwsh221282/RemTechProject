using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Worker.Service.Contracts.BaseContracts;

namespace WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

public sealed class ClearPoolBehavior : IWebDriverBehavior
{
    public async Task<Result> Execute(IMessagePublisher publisher, CancellationToken ct = default)
    {
        var result = await publisher.Send(new ClearPoolContract(), ct);
        return result.ToResult();
    }
}

public sealed class ClearClientPoolBehavior : IWebDriverBehavior
{
    private readonly WebElementPool _pool;

    public ClearClientPoolBehavior(WebElementPool pool) => _pool = pool;

    public async Task<Result> Execute(IMessagePublisher publisher, CancellationToken ct = default)
    {
        _pool.Clear();
        return await Task.FromResult(Result.Success());
    }
}

public sealed class ClearWebElementChilds : IWebDriverBehavior
{
    private readonly WebElement _element;

    public ClearWebElementChilds(WebElement element) => _element = element;

    public async Task<Result> Execute(IMessagePublisher publisher, CancellationToken ct = default)
    {
        _element.ClearChilds();
        return await Task.FromResult(Result.Success());
    }
}
