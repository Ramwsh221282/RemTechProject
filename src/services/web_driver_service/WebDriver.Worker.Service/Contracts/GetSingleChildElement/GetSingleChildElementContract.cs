using Rabbit.RPC.Server.Abstractions.Communication;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Application;
using WebDriver.Application.DTO;
using WebDriver.Application.Queries.GetElementInsideOfElement;
using WebDriver.Core.Models;
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
        ExistingElementDTO existing = new(contract.ParentId);
        ElementPathDataDTO data = new(contract.Path, contract.Type);
        GetElementInsideOfElementQuery query = new(existing, data);

        Result<WebElementObject> element = await _api.ExecuteQuery<
            GetElementInsideOfElementQuery,
            WebElementObject
        >(query);

        if (element.IsFailure)
            return new(element.Error.Description);

        GetElementResponse response =
            new(element.Value.ElementPath, element.Value.ElementPathType, element.Value.ElementId);
        return new ContractActionResult(response);
    }
}
