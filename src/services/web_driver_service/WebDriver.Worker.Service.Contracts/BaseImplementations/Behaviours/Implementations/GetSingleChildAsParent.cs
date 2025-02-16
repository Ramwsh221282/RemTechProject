using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Worker.Service.Contracts.BaseContracts;
using WebDriver.Worker.Service.Contracts.Responses;

namespace WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

public sealed class GetSingleChildAsParent(
    WebElement parent,
    WebElementPool pool,
    string path,
    string pathType,
    string childName
) : IWebDriverBehavior
{
    public async Task<Result> Execute(IMessagePublisher publisher, CancellationToken ct = default)
    {
        ContractActionResult request = await publisher.Send(
            new GetSingleChildElementContract(parent, path, pathType),
            ct
        );

        Result result = request.ToResult();
        if (result.IsFailure)
            return result;

        WebElementResponse response = request.FromResult<WebElementResponse>();
        WebElement element = new(response, childName);
        pool.AddElement(element);
        return Result.Success();
    }
}
