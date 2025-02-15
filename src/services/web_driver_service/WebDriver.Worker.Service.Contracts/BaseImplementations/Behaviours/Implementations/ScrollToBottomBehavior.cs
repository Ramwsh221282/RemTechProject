using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Worker.Service.Contracts.BaseContracts;
using WebDriver.Worker.Service.Contracts.Utils;

namespace WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

public class ScrollToBottomBehavior : IWebDriverBehavior
{
    public virtual async Task<Result> Execute(
        IMessagePublisher publisher,
        CancellationToken ct = default
    )
    {
        var result = await publisher.Send(new ScrollPageDownContract(), ct);
        return result.ToResult();
    }
}

public sealed class ScrollToBottomRetriable(byte maxAttempts) : ScrollToBottomBehavior, IRetriable
{
    public byte MaxAttempts { get; } = maxAttempts;

    public override async Task<Result> Execute(
        IMessagePublisher publisher,
        CancellationToken ct = default
    )
    {
        int attempt = 0;
        while (attempt < MaxAttempts)
        {
            Result execution = await base.Execute(publisher, ct);
            if (execution.IsSuccess)
                return execution;

            await Task.Delay(TimeSpan.FromSeconds(1));
            attempt++;
        }

        return new Error($"Cannot execute scroll bottom. Attempts : {MaxAttempts}");
    }
}
