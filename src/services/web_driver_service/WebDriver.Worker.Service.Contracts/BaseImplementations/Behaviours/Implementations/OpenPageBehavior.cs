using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Worker.Service.Contracts.BaseContracts;

namespace WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

public sealed class OpenPageBehavior(string pageUrl) : IWebDriverBehavior
{
    private int _waitTimeInSeconds;

    public async Task<Result> Execute(IMessagePublisher publisher, CancellationToken ct = default)
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
