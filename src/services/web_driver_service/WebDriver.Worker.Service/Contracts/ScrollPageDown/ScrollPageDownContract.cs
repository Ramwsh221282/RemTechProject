using Rabbit.RPC.Server.Abstractions.Communication;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Application;
using WebDriver.Application.Commands.ScrollToDown;

namespace WebDriver.Worker.Service.Contracts.ScrollPageDown;

internal sealed record ScrollPageDownContract : IContract;

internal sealed class ScrollPageDownContractHandler(WebDriverApi api)
    : IContractHandler<ScrollPageDownContract>
{
    public async Task<ContractActionResult> Handle(ScrollPageDownContract contract)
    {
        ScrollToDownCommand command = new ScrollToDownCommand();
        Result scrolling = await api.ExecuteCommand(command);

        return scrolling.IsFailure
            ? ContractActionResult.Fail(scrolling.Error.Description)
            : ContractActionResult.Success(true);
    }
}
