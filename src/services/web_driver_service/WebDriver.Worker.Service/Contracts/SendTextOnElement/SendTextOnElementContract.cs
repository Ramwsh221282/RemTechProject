using Rabbit.RPC.Server.Abstractions.Communication;
using WebDriver.Application;
using WebDriver.Application.Commands.SendTextOnElement;
using WebDriver.Application.DTO;

namespace WebDriver.Worker.Service.Contracts.SendTextOnElement;

internal sealed record SendTextOnElementContract(Guid ElementId, string Text) : IContract;

internal sealed class SendTextOnElementContractHandler(WebDriverApi api)
    : IContractHandler<SendTextOnElementContract>
{
    public async Task<ContractActionResult> Handle(SendTextOnElementContract contract)
    {
        ExistingElementDTO dto = new(contract.ElementId);
        string text = contract.Text;
        SendTextOnElementCommand command = new(dto, text);
        var sending = await api.ExecuteCommand(command);

        return sending.IsFailure
            ? ContractActionResult.Fail(sending.Error.Description)
            : ContractActionResult.Success(true);
    }
}
