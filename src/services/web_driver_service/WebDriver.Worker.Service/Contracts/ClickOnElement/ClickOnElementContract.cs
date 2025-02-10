using Rabbit.RPC.Server.Abstractions.Communication;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Application;
using WebDriver.Application.Commands.ClickOnElement;
using WebDriver.Application.DTO;

namespace WebDriver.Worker.Service.Contracts.ClickOnElement;

internal sealed record ClickOnElementContract(Guid ExistingId) : IContract;

internal sealed record ClickOnElementResponse(bool IsClicked);

internal sealed class ClickOnElementContractHandler : IContractHandler<ClickOnElementContract>
{
    private readonly WebDriverApi _api;

    public ClickOnElementContractHandler(WebDriverApi api) => _api = api;

    public async Task<ContractActionResult> Handle(ClickOnElementContract contract)
    {
        ExistingElementDTO dto = new(contract.ExistingId);
        ClickOnElementCommand command = new(dto);

        Result clicking = await _api.ExecuteCommand(command);
        if (clicking.IsFailure)
            return new ContractActionResult(clicking.Error.Description);

        ClickOnElementResponse response = new(true);
        return new ContractActionResult(response);
    }
}
