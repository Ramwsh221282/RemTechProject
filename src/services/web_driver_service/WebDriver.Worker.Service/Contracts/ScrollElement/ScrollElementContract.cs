using Rabbit.RPC.Server.Abstractions.Communication;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Application;
using WebDriver.Application.Commands.ScrollElement;
using WebDriver.Application.DTO;

namespace WebDriver.Worker.Service.Contracts.ScrollElement;

internal sealed record ScrollElementContract(Guid ExistingId) : IContract;

internal sealed class ScrollElementContractHandler(WebDriverApi api)
    : IContractHandler<ScrollElementContract>
{
    public async Task<ContractActionResult> Handle(ScrollElementContract contract)
    {
        ExistingElementDTO dto = new(contract.ExistingId);
        ScrollElementCommand command = new(dto);

        Result scrolling = await api.ExecuteCommand(command);
        return scrolling.IsFailure
            ? ContractActionResult.Fail(scrolling.Error.Description)
            : ContractActionResult.Success(true);
    }
}
