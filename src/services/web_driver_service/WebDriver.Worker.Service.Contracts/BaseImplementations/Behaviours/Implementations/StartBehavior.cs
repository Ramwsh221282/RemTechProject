using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Worker.Service.Contracts.BaseContracts;
using WebDriver.Worker.Service.Contracts.Utils;

namespace WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

public class StartBehavior(string startStrategy) : IWebDriverBehavior
{
    public async Task<Result> Execute(IMessagePublisher publisher, CancellationToken ct = default)
    {
        var result = await publisher.Send(new StartWebDriverContract(startStrategy), ct);
        return result.ToResult();
    }
}

public sealed class StartRetriable(string startStrategy, byte attemptsLimit)
    : IWebDriverBehavior,
        IRetriable
{
    public byte MaxAttempts { get; } = attemptsLimit;
    private readonly StartBehavior start = new StartBehavior(startStrategy);

    public async Task<Result> Execute(IMessagePublisher publisher, CancellationToken ct = default)
    {
        byte attempts = 0;
        while (attempts < MaxAttempts)
        {
            Result starting = await start.Execute(publisher, ct);
            if (starting.IsSuccess)
                return starting;

            await Task.Delay(TimeSpan.FromSeconds(1), ct);
            attempts++;
        }

        return new Error("Cannot start web driver instance");
    }
}
