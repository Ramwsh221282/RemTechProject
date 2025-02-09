using Rabbit.RPC.Server.Abstractions.Communication;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Core.Core;
using WebDriver.Core.Queries.GetElement;
using WebDriver.Core.Queries.GetElementInsideOfElement;
using WebDriver.Worker.Service.Contracts.GetSingleElement;

namespace WebDriver.Worker.Service.Contracts.GetSingleChildElement;

internal sealed record GetSingleChildElementContract(Guid ParentId, string Path, string Type)
    : IContract;

internal sealed class GetSingleChildElementContractHandler
    : IContractHandler<GetSingleChildElementContract>
{
    private readonly WebDriverApi _api;

    public GetSingleChildElementContractHandler(WebDriverApi api) => _api = api;

    public async Task<ContractActionResult> Handle(GetSingleChildElementContract contract)
    {
        Result<GetElementQuery> queryModel = GetElementQueryFactory.Create(
            contract.Path,
            contract.Type
        );
        if (queryModel.IsFailure)
            return new(queryModel.Error.Description);

        GetElementInsideOfElementQuery query = new(contract.ParentId, queryModel);
        Result<WebElementObject> child = await _api.ExecuteQuery<
            GetElementInsideOfElementQuery,
            WebElementObject
        >(query);

        if (child.IsFailure)
            return new(child.Error.Description);

        GetElementResponse response =
            new(child.Value.ElementPath, child.Value.ElementPathType, child.Value.ElementId);
        return new ContractActionResult(response);
    }
}
