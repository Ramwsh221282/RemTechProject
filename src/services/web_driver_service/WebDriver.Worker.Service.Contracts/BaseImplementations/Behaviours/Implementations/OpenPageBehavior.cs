using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Worker.Service.Contracts.BaseContracts;
using WebDriver.Worker.Service.Contracts.Utils;

namespace WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

public class OpenPageBehavior(string pageUrl) : IWebDriverBehavior
{
    private int _waitTimeInSeconds;

    public virtual async Task<Result> Execute(
        IMessagePublisher publisher,
        CancellationToken ct = default
    )
    {
        var result = await publisher.Send(new OpenWebDriverPageContract(pageUrl), ct);
        await Task.Delay(TimeSpan.FromSeconds(_waitTimeInSeconds), ct);
        return result.ToResult();
    }

    public OpenPageBehavior WithWait(int waitTimeInSeconds)
    {
        _waitTimeInSeconds = waitTimeInSeconds;
        return this;
    }
}

public sealed class OpenPageRepeatable(string pageUrl, byte maxAttemprs)
    : OpenPageBehavior(pageUrl),
        IRetriable
{
    public byte MaxAttempts { get; } = maxAttemprs;

    public override async Task<Result> Execute(
        IMessagePublisher publisher,
        CancellationToken ct = default
    )
    {
        byte attempts = 0;
        while (attempts < MaxAttempts)
        {
            Result result = await base.Execute(publisher, ct);
            if (result.IsSuccess)
                return result;

            attempts++;
            await Task.Delay(TimeSpan.FromSeconds(1));
        }

        return new Error($"Cannot open page. Attempts reached: {MaxAttempts}");
    }
}
