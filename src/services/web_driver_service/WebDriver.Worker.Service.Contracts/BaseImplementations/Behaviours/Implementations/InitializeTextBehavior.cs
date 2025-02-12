using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Worker.Service.Contracts.BaseContracts;

namespace WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

public sealed class InitializeTextBehavior(WebElement element) : IWebDriverBehavior
{
    public async Task<Result> Execute(IMessagePublisher publisher, CancellationToken ct = default)
    {
        var result = await publisher.Send(
            new GetTextFromElementContract(element.Model.ElementId),
            ct
        );

        string text = result.FromResult<string>();
        element.SetText(text);
        return Result.Success();
    }
}
