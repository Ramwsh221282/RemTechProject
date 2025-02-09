using Rabbit.RPC.Server.Abstractions.Communication;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Core.Core;
using WebDriver.Core.Queries.GetElement;
using WebDriver.Core.Queries.GetElementsInsideOfElement;
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
        Result<GetElementQuery> queryModel = GetElementQueryFactory.Create(
            contract.Path,
            contract.Type
        );
        if (queryModel.IsFailure)
            return new ContractActionResult(queryModel.Error.Description);

        GetElementsInsideOfElementQuery query = new(contract.ParentId, queryModel);
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
