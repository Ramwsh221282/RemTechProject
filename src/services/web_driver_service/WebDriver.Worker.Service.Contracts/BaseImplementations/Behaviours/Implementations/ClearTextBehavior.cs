using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Worker.Service.Contracts.BaseContracts;

namespace WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

public class ClearTextBehavior : IWebDriverBehavior
{
    private readonly WebElement _element;

    public ClearTextBehavior(WebElement element) => _element = element;

    public async Task<Result> Execute(IMessagePublisher publisher, CancellationToken ct = default)
    {
        var result = await publisher.Send(new ClearTextContract(_element), ct);
        return result.ToResult();
    }
}
