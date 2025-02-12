using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Worker.Service.Contracts.BaseContracts;
using WebDriver.Worker.Service.Contracts.Responses;

namespace WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

public sealed class GetSingleElementBehavior(
    WebElementPool pool,
    string path,
    string pathType,
    string name
) : IWebDriverBehavior
{
    public async Task<Result> Execute(IMessagePublisher publisher, CancellationToken ct = default)
    {
        var request = await publisher.Send(new GetSingleElementContract(path, pathType), ct);
        WebElementResponse response = request.FromResult<WebElementResponse>();
        Result result = request.ToResult();

        if (result.IsFailure)
            return result;

        WebElement element = new(response, name);
        pool.AddElement(element);
        return result;
    }
}
