using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;

namespace WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

public abstract class DoForParent : IWebDriverBehavior
{
    protected readonly WebElementPool _pool;
    protected readonly string _parentName;
    protected readonly Func<WebElement, IWebDriverBehavior>[] _behaviorFactories;

    protected DoForParent(
        WebElementPool pool,
        string parentName,
        params Func<WebElement, IWebDriverBehavior>[] behaviorFactories
    )
    {
        _pool = pool;
        _parentName = parentName;
        _behaviorFactories = behaviorFactories;
    }

    public abstract Task<Result> Execute(
        IMessagePublisher publisher,
        CancellationToken ct = default
    );
}

public sealed class DoForExactParent : DoForParent
{
    public DoForExactParent(
        WebElementPool pool,
        string parentName,
        params Func<WebElement, IWebDriverBehavior>[] behaviorFactories
    )
        : base(pool, parentName, behaviorFactories) { }

    public override async Task<Result> Execute(
        IMessagePublisher publisher,
        CancellationToken ct = default
    )
    {
        Result<WebElement> element = _pool.GetWebElement(el => el.Name == _parentName);
        if (element.IsFailure)
            return element.Error;

        foreach (var factory in _behaviorFactories)
        {
            IWebDriverBehavior behavior = factory.Invoke(element.Value);
            Result execution = await behavior.Execute(publisher, ct);
            if (execution.IsFailure)
                return execution;
        }

        return Result.Success();
    }
}

public sealed class DoForSpecificParents : DoForParent
{
    private readonly Func<WebElement, bool> _predicate;

    public DoForSpecificParents(
        WebElementPool pool,
        Func<WebElement, bool> predicate,
        params Func<WebElement, IWebDriverBehavior>[] behaviorFactories
    )
        : base(pool, string.Empty, behaviorFactories) => _predicate = predicate;

    public override async Task<Result> Execute(
        IMessagePublisher publisher,
        CancellationToken ct = default
    )
    {
        IEnumerable<WebElement> parents = _pool.GetWebElements(_predicate);
        foreach (var parent in parents)
        {
            foreach (var factory in _behaviorFactories)
            {
                IWebDriverBehavior behavior = factory.Invoke(parent);
                Result execution = await behavior.Execute(publisher, ct);
                if (execution.IsFailure)
                    return execution;
            }
        }

        return Result.Success();
    }
}
