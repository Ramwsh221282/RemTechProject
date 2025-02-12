using Rabbit.RPC.Server.Abstractions.Communication;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Application;
using WebDriver.Application.Commands.ClearText;
using WebDriver.Application.DTO;

namespace WebDriver.Worker.Service.Contracts.ClearText;

internal sealed record ClearTextContract(Guid ExistingId) : IContract;

internal sealed class ClearTextContractHandler(WebDriverApi api)
    : IContractHandler<ClearTextContract>
{
    public async Task<ContractActionResult> Handle(ClearTextContract contract)
    {
        ExistingElementDTO dto = new(contract.ExistingId);
        ClearTextCommand command = new(dto);
        Result clearing = await api.ExecuteCommand(command);

        return clearing.IsFailure
            ? ContractActionResult.Fail(clearing.Error.Description)
            : ContractActionResult.Success(true);
    }
}
