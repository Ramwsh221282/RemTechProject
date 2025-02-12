using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Worker.Service.Contracts.BaseContracts;

namespace WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

public sealed class ScrollElementBehavior(WebElement element) : IWebDriverBehavior
{
    public async Task<Result> Execute(IMessagePublisher publisher, CancellationToken ct = default)
    {
        ContractActionResult scrolling = await publisher.Send(
            new ScrollElementContract(element.Model.ElementId),
            ct
        );
        return scrolling.ToResult();
    }
}
