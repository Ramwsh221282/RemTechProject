using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Worker.Service.Contracts.Responses;

namespace WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

public sealed class InitializeAttributeBehavior(WebElement element, string attributeName)
    : IWebDriverBehavior
{
    public async Task<Result> Execute(IMessagePublisher publisher, CancellationToken ct = default)
    {
        var request = await publisher.Send(
            new GetElementAttributeValueContract(element.Model, attributeName),
            ct
        );

        string value = request.FromResult<string>();
        if (!string.IsNullOrWhiteSpace(value))
            element.SetAttribute(attributeName, value);

        return Result.Success();
    }
}
