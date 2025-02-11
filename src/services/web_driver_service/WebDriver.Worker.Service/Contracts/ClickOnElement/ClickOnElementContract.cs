using Rabbit.RPC.Server.Abstractions.Communication;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Application;
using WebDriver.Application.Commands.ClickOnElement;
using WebDriver.Application.DTO;

namespace WebDriver.Worker.Service.Contracts.ClickOnElement;

internal sealed record ClickOnElementContract(Guid ExistingId) : IContract;

internal sealed class ClickOnElementContractHandler(WebDriverApi api)
    : IContractHandler<ClickOnElementContract>
{
    public async Task<ContractActionResult> Handle(ClickOnElementContract contract)
    {
        ExistingElementDTO dto = new(contract.ExistingId);
        ClickOnElementCommand command = new(dto);

        Result clicking = await api.ExecuteCommand(command);
        return clicking.IsFailure
            ? ContractActionResult.Fail(clicking.Error.Description)
            : ContractActionResult.Success(true);
    }
}
