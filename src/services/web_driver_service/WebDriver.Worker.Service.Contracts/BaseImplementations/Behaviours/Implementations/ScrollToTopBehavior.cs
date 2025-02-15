using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Worker.Service.Contracts.BaseContracts;
using WebDriver.Worker.Service.Contracts.Utils;

namespace WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

public class ScrollToTopBehavior : IWebDriverBehavior
{
    public virtual async Task<Result> Execute(
        IMessagePublisher publisher,
        CancellationToken ct = default
    )
    {
        var result = await publisher.Send(new ScrollPageTopContract(), ct);
        return result.ToResult();
    }
}

public sealed class ScrollToTopRetriable(byte maxAttempts) : ScrollToTopBehavior, IRetriable
{
    public override async Task<Result> Execute(
        IMessagePublisher publisher,
        CancellationToken ct = default
    )
    {
        int attempts = 0;
        while (attempts < MaxAttempts)
        {
            Result execution = await base.Execute(publisher, ct);
            if (execution.IsSuccess)
                return execution;

            await Task.Delay(TimeSpan.FromSeconds(1));
            attempts++;
        }

        return new Error($"Cannot execute scroll top. Attempts: {attempts}");
    }

    public byte MaxAttempts { get; } = maxAttempts;
}
