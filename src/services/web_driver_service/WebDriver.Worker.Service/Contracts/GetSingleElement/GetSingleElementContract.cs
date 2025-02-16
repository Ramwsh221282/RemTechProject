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

        Result<WebElementResponseObject> element = await api.ExecuteQuery<
            GetElementQuery,
            WebElementResponseObject
        >(query);
        if (element.IsFailure)
            return ContractActionResult.Fail(element.Error.Description);

        WebElementResponse response = element.Value;
        return ContractActionResult.Success(response);
    }
}
