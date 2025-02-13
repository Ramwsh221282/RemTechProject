using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Worker.Service.Contracts.BaseContracts;

namespace WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

public sealed class CompositeBehavior(ILogger? logger = null) : IWebDriverBehavior
{
    private readonly Queue<IWebDriverBehavior> _behaviors = new Queue<IWebDriverBehavior>();

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

    public async Task<Result> Execute(IMessagePublisher publisher, CancellationToken ct = default)
    {
        while (_behaviors.Count != 0)
        {
            IWebDriverBehavior behavior = _behaviors.Dequeue();
            Result execution = await behavior.Execute(publisher, ct);
            if (execution.IsFailure)
            {
                logger?.Error("Operation {Name} resulted in failure", behavior.GetType().Name);
                await publisher.Send(new StopWebDriverContract(), ct);
                return execution.Error;
            }

            string typeName = behavior.GetType().Name;
            logger?.Information("Operation {Name} resulted in success", typeName);
        }

        logger?.Information("Pipeline finished");
        return Result.Success();
    }
}
