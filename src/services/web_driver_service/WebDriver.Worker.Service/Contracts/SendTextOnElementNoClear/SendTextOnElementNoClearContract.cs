using Rabbit.RPC.Server.Abstractions.Communication;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Application;
using WebDriver.Application.Commands.SendTextOnElementWithoutClear;
using WebDriver.Application.DTO;

namespace WebDriver.Worker.Service.Contracts.SendTextOnElementNoClear;

internal sealed record SendTextOnElementNoClearContract(Guid ElementId, string Text) : IContract;

internal sealed class SendTextOnElementNoClearContractHandler(WebDriverApi api)
    : IContractHandler<SendTextOnElementNoClearContract>
{
    public async Task<ContractActionResult> Handle(SendTextOnElementNoClearContract contract)
    {
        ExistingElementDTO data = new(contract.ElementId);
        string text = contract.Text;
        SendTextOnElementWithoutClearCommand command = new(data, text);

        Result writing = await api.ExecuteCommand(command);
        return writing.IsFailure
            ? ContractActionResult.Fail(writing.Error.Description)
            : ContractActionResult.Success(true);
    }
}
