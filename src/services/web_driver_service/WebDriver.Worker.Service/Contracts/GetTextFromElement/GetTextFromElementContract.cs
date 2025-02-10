using Rabbit.RPC.Server.Abstractions.Communication;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Application;
using WebDriver.Application.DTO;
using WebDriver.Application.Queries.GetTextFromElement;

namespace WebDriver.Worker.Service.Contracts.GetTextFromElement;

internal sealed record GetTextFromElementContract(Guid ExistingId) : IContract;

internal sealed record GetTextFromElementResponse(string Text);

internal sealed class GetTextFromElementContractHandler
    : IContractHandler<GetTextFromElementContract>
{
    private readonly WebDriverApi _api;

    public GetTextFromElementContractHandler(WebDriverApi api) => _api = api;

    public async Task<ContractActionResult> Handle(GetTextFromElementContract contract)
    {
        ExistingElementDTO existing = new(contract.ExistingId);
        GetTextFromElementQuery query = new(existing);

        Result<string> result = await _api.ExecuteQuery<GetTextFromElementQuery, string>(query);
        if (result.IsFailure)
            return new ContractActionResult(result.Error.Description);

        GetTextFromElementResponse response = new(result.Value);
        return new ContractActionResult(response);
    }
}
