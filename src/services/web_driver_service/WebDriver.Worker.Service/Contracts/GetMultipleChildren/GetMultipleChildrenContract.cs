using Rabbit.RPC.Server.Abstractions.Communication;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Application;
using WebDriver.Application.DTO;
using WebDriver.Application.Queries.GetElementsInsideOfElement;
using WebDriver.Core.Models;
using WebDriver.Worker.Service.Contracts.GetSingleElement;

namespace WebDriver.Worker.Service.Contracts.GetMultipleChildren;

internal sealed record GetMultipleChildrenContract(Guid ParentId, string Path, string Type)
    : IContract;

internal sealed record GetMultipleChildrenResponse(GetElementResponse[] Results);

internal sealed class GetMultipleChildrenContractHandler
    : IContractHandler<GetMultipleChildrenContract>
{
    private readonly WebDriverApi _api;

    public GetMultipleChildrenContractHandler(WebDriverApi api) => _api = api;

    public async Task<ContractActionResult> Handle(GetMultipleChildrenContract contract)
    {
        ExistingElementDTO existing = new(contract.ParentId);
        ElementPathDataDTO path = new(contract.Path, contract.Type);

        GetElementsInsideOfElementQuery query = new(existing, path);
        Result<WebElementObject[]> result = await _api.ExecuteQuery<
            GetElementsInsideOfElementQuery,
            WebElementObject[]
        >(query);

        if (result.IsFailure)
            return new ContractActionResult(result.Error.Description);

        GetElementResponse[] items = result
            .Value.Select(i => new GetElementResponse(
                i.ElementPath,
                i.ElementPathType,
                i.ElementId
            ))
            .ToArray();

        GetMultipleChildrenResponse response = new(items);
        return new ContractActionResult(response);
    }
}
