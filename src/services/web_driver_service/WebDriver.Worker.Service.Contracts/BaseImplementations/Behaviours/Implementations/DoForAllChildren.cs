using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;

namespace WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

public abstract class DoForChildren : IWebDriverBehavior
{
    protected readonly WebElementPool _pool;
    protected readonly string _parentName;
    protected readonly Func<WebElement, IWebDriverBehavior>[] _factories;

    protected DoForChildren(
        WebElementPool pool,
        string parentName,
        params Func<WebElement, IWebDriverBehavior>[] factories
    )
    {
        _pool = pool;
        _parentName = parentName;
        _factories = factories;
    }

    public abstract Task<Result> Execute(
        IMessagePublisher publisher,
        CancellationToken ct = default
    );
}

public sealed class DoForAllChildren : DoForChildren
{
    public DoForAllChildren(
        WebElementPool pool,
        string parentName,
        params Func<WebElement, IWebDriverBehavior>[] factories
    )
        : base(pool, parentName, factories) { }

    public override async Task<Result> Execute(
        IMessagePublisher publisher,
        CancellationToken ct = default
    )
    {
        Result<WebElement> root = _pool.GetWebElement(el => el.Name == _parentName);
        if (root.IsFailure)
            return root.Error;

        foreach (var factory in _factories)
        {
            await root.Value.ExecuteForChilds(publisher, factory, ct);
        }

        return Result.Success();
    }
}

public sealed class DoForSpecificChildren : DoForChildren
{
    private readonly Func<WebElement, bool> _predicate;

    public DoForSpecificChildren(
        WebElementPool pool,
        string parentName,
        Func<WebElement, bool> predicate,
        params Func<WebElement, IWebDriverBehavior>[] factories
    )
        : base(pool, parentName, factories)
    {
        _predicate = predicate;
    }

    public override async Task<Result> Execute(
        IMessagePublisher publisher,
        CancellationToken ct = default
    )
    {
        Result<WebElement> root = _pool.GetWebElement(el => el.Name == _parentName);
        if (root.IsFailure)
            return root.Error;

        foreach (var factory in _factories)
        {
            await root.Value.ExecuteForChilds(publisher, factory, _predicate, ct);
        }

        return Result.Success();
    }
}
