using Rabbit.RPC.Server.Abstractions.Communication;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Application;
using WebDriver.Application.DTO;
using WebDriver.Application.Queries.GetTextFromElement;

namespace WebDriver.Worker.Service.Contracts.GetTextFromElement;

internal sealed record GetTextFromElementContract(Guid ExistingId) : IContract;

internal sealed class GetTextFromElementContractHandler(WebDriverApi api)
    : IContractHandler<GetTextFromElementContract>
{
    public async Task<ContractActionResult> Handle(GetTextFromElementContract contract)
    {
        ExistingElementDTO existing = new(contract.ExistingId);
        GetTextFromElementQuery query = new(existing);

        Result<string> result = await api.ExecuteQuery<GetTextFromElementQuery, string>(query);

        return result.IsFailure
            ? ContractActionResult.Fail(result.Error.Description)
            : ContractActionResult.Success(result.Value);
    }
}
