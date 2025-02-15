using Rabbit.RPC.Server.Abstractions.Communication;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Application;
using WebDriver.Application.DTO;
using WebDriver.Application.Queries.GetElement;
using WebDriver.Core.Models;
using WebDriver.Worker.Service.Responses;

namespace WebDriver.Worker.Service.Contracts.GetSingleElement;

internal record GetSingleElementContract(string ElementPath, string ElementPathType) : IContract;

internal sealed class GetSingleElementContractHandler(WebDriverApi api)
    : IContractHandler<GetSingleElementContract>
{
    public async Task<ContractActionResult> Handle(GetSingleElementContract contract)
    {
        ElementPathDataDTO data = new(contract.ElementPath, contract.ElementPathType);
        GetElementQuery query = new(data);

        Result<WebElementObject> element = await api.ExecuteQuery<
            GetElementQuery,
            WebElementObject
        >(query);

        WebElementResponse response = new WebElementResponse(
            element.Value.ElementId,
            element.Value.ElementOuterHTMLBytes,
            element.Value.ElementInnerTextBytes
        );

        return element.IsFailure
            ? ContractActionResult.Fail(element.Error.Description)
            : ContractActionResult.Success(response);
    }
}
