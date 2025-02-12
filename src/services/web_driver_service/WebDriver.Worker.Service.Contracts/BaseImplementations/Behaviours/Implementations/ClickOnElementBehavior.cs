using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Worker.Service.Contracts.BaseContracts;

namespace WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

public sealed class ClickOnElementBehavior(WebElement element) : IWebDriverBehavior
{
    private int _waitTimeInSeconds;

    public async Task<Result> Execute(IMessagePublisher publisher, CancellationToken ct = default)
    {
        var clicking = await publisher.Send(new ClickOnElementContract(element.Model), ct);
        await Task.Delay(TimeSpan.FromSeconds(_waitTimeInSeconds), ct);
        return clicking.ToResult();
    }

    public ClickOnElementBehavior WithWait(int waitTimeInSeconds)
    {
        _waitTimeInSeconds = waitTimeInSeconds;
        return this;
    }
}
