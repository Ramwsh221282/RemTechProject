using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

public sealed class CompositeBehavior(ILogger? logger = null) : IWebDriverBehavior
{
    private readonly List<IWebDriverBehavior> _behaviors = [];

    public CompositeBehavior AddBehavior(params IWebDriverBehavior[] behaviors)
    {
        foreach (IWebDriverBehavior behavior in behaviors)
        {
            _behaviors.Add(behavior);
            if (logger == null)
                return this;
            string typeName = behavior.GetType().Name;
            logger.Information("Added operation {Name} in pipeLine", typeName);
        }

        return this;
    }

    public CompositeBehavior AddBehavior(IWebDriverBehavior behavior)
    {
        _behaviors.Add(behavior);
        if (logger == null)
            return this;
        string typeName = behavior.GetType().Name;
        logger.Information("Added operation {Name} in pipeLine", typeName);
        return this;
    }

    public async Task<Result> Execute(IMessagePublisher publisher, CancellationToken ct = default)
    {
        foreach (IWebDriverBehavior behavior in _behaviors)
        {
            Result execution = await behavior.Execute(publisher, ct);
            if (logger == null)
                continue;
            string typeName = behavior.GetType().Name;
            if (execution.IsFailure)
                logger.Error("Operation {Name} resulted in failure", typeName);
            if (execution.IsSuccess)
                logger.Information("Operation {Name} resulted in success", typeName);
        }
        logger?.Information("Pipeline finished");
        return Result.Success();
    }
}
