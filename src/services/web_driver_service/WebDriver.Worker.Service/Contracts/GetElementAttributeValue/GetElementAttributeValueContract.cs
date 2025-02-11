using Rabbit.RPC.Server.Abstractions.Communication;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Application;
using WebDriver.Application.DTO;
using WebDriver.Application.Queries.GetElementAttribute;

namespace WebDriver.Worker.Service.Contracts.GetElementAttributeValue;

internal sealed record GetElementAttributeValueContract(Guid ElementId, string AttributeName)
    : IContract;

internal sealed class GetElementAttributeValueContractHandler(WebDriverApi api)
    : IContractHandler<GetElementAttributeValueContract>
{
    public async Task<ContractActionResult> Handle(GetElementAttributeValueContract contract)
    {
        ExistingElementDTO existing = new(contract.ElementId);
        ElementAttributeDTO attribute = new(contract.AttributeName);
        GetElementAttributeQuery query = new(existing, attribute);

        Result<string> value = await api.ExecuteQuery<GetElementAttributeQuery, string>(query);
        return value.IsFailure
            ? ContractActionResult.Fail(value.Error.Description)
            : ContractActionResult.Success(value.Value);
    }
}
