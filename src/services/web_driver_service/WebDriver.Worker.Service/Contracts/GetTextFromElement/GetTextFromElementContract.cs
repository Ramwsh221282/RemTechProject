using Rabbit.RPC.Server.Abstractions.Communication;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Core.Core;
using WebDriver.Core.Queries.GetTextFromElement;

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
        GetTextFromElementQuery query = new(contract.ExistingId);
        Result<GetTextFromElementQueryResult> result = await _api.ExecuteQuery<
            GetTextFromElementQuery,
            GetTextFromElementQueryResult
        >(query);

        if (result.IsFailure)
            return new ContractActionResult(result.Error.Description);

        GetTextFromElementResponse response = new(result.Value.Text);
        return new ContractActionResult(response);
    }
}
