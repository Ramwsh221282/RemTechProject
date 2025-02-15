using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

public sealed class CompositeBehavior(ILogger? logger = null) : IWebDriverBehavior
{
    private readonly Queue<IWebDriverBehavior> _behaviors = new Queue<IWebDriverBehavior>();

    private IWebDriverBehavior? _onFinish;

    public CompositeBehavior AddBehavior(params IWebDriverBehavior[] behaviors)
    {
        foreach (IWebDriverBehavior behavior in behaviors)
        {
            _behaviors.Enqueue(behavior);
            if (logger == null)
                return this;
            string typeName = behavior.GetType().Name;
            logger.Information("Added operation {Name} in pipeLine", typeName);
        }

        return this;
    }

    public CompositeBehavior AddBehavior(IWebDriverBehavior behavior)
    {
        _behaviors.Enqueue(behavior);
        if (logger == null)
            return this;
        string typeName = behavior.GetType().Name;
        logger.Information("Added operation {Name} in pipeLine", typeName);
        return this;
    }

    public CompositeBehavior AddOnFinish(IWebDriverBehavior behavior)
    {
        _onFinish = behavior;
        return this;
    }

    public async Task<Result> Execute(IMessagePublisher publisher, CancellationToken ct = default)
    {
        while (_behaviors.Count != 0)
        {
            IWebDriverBehavior behavior = _behaviors.Dequeue();
            string typeName = behavior.GetType().Name;
            Result execution = await behavior.Execute(publisher, ct);

            if (execution.IsFailure)
                logger?.Error("Operation {Name} resulted in failure", typeName);

            logger?.Information("Operation {Name} resulted in success", typeName);
        }

        if (_onFinish != null)
            await _onFinish.Execute(publisher, ct);

        logger?.Information("Pipeline finished");
        return Result.Success();
    }
}
