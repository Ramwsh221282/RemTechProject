using Rabbit.RPC.Server.Abstractions.Communication;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Application;
using WebDriver.Application.DTO;
using WebDriver.Application.Queries.GetElement;
using WebDriver.Core.Models;

namespace WebDriver.Worker.Service.Contracts.GetSingleElement;

internal record GetSingleElementContract(string ElementPath, string ElementPathType) : IContract;

internal record GetElementResponse(string ElementPath, string ElementPathType, Guid ElementId);

internal sealed class GetSingleElementContractHandler : IContractHandler<GetSingleElementContract>
{
    private readonly WebDriverApi _api;

    public GetSingleElementContractHandler(WebDriverApi api) => _api = api;

    public async Task<ContractActionResult> Handle(GetSingleElementContract contract)
    {
        ElementPathDataDTO data = new(contract.ElementPath, contract.ElementPathType);
        GetElementQuery query = new(data);

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
