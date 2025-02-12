using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Worker.Service.Contracts.BaseContracts;
using WebDriver.Worker.Service.Contracts.Responses;

namespace WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

public sealed class GetChildrenBehavior(
    WebElement element,
    string childsName,
    string path,
    string pathType
) : IWebDriverBehavior
{
    public async Task<Result> Execute(IMessagePublisher publisher, CancellationToken ct = default)
    {
        var getChildren = await publisher.Send(
            new GetMultipleChildrenContract(element.Model.ElementId, path, pathType),
            ct
        );

        var result = getChildren.ToResult();
        if (result.IsFailure)
            return result;

        WebElementResponse[] children = getChildren.FromResult<WebElementResponse[]>();
        element.FromParent(children, childsName);
        return result;
    }
}
