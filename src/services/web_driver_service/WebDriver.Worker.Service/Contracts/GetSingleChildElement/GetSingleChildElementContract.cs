using Rabbit.RPC.Server.Abstractions.Communication;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Application;
using WebDriver.Application.DTO;
using WebDriver.Application.Queries.GetElementInsideOfElement;
using WebDriver.Core.Models;
using WebDriver.Worker.Service.Responses;

namespace WebDriver.Worker.Service.Contracts.GetSingleChildElement;

internal sealed record GetSingleChildElementContract(Guid ParentId, string Path, string Type)
    : IContract;

internal sealed class GetSingleChildElementContractHandler(WebDriverApi api)
    : IContractHandler<GetSingleChildElementContract>
{
    public async Task<ContractActionResult> Handle(GetSingleChildElementContract contract)
    {
        ExistingElementDTO existing = new(contract.ParentId);
        ElementPathDataDTO data = new(contract.Path, contract.Type);
        GetElementInsideOfElementQuery query = new(existing, data);

        Result<WebElementObject> element = await api.ExecuteQuery<
            GetElementInsideOfElementQuery,
            WebElementObject
        >(query);

        WebElementResponse response =
            new(element.Value.ElementPath, element.Value.ElementPathType, element.Value.ElementId);

        return element.IsFailure
            ? ContractActionResult.Fail(element.Error.Description)
            : ContractActionResult.Success(response);
    }
}
