using Rabbit.RPC.Server.Abstractions.Communication;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Core.Core;
using WebDriver.Core.Queries.GetElement;

namespace WebDriver.Worker.Service.Contracts.GetSingleElement;

internal record GetSingleElementContract(string ElementPath, string ElementPathType) : IContract;

internal record GetElementResponse(string ElementPath, string ElementPathType, Guid ElementId);

internal sealed class GetSingleElementContractHandler : IContractHandler<GetSingleElementContract>
{
    private readonly WebDriverApi _api;

    public GetSingleElementContractHandler(WebDriverApi api) => _api = api;

    public async Task<ContractActionResult> Handle(GetSingleElementContract contract)
    {
        Result<GetElementQuery> query = GetElementQueryFactory.Create(
            contract.ElementPath,
            contract.ElementPathType
        );
        if (query.IsFailure)
            return new ContractActionResult(query.Error.Description);

        Result<WebElementObject> element = await _api.ExecuteQuery<
            GetElementQuery,
            WebElementObject
        >(query);
        if (element.IsFailure)
            return new ContractActionResult(element.Error.Description);

        GetElementResponse response =
            new(element.Value.ElementPath, element.Value.ElementPathType, element.Value.ElementId);
        ContractActionResult result = new(response);
        return result;
    }
}
