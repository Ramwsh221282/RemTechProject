using Rabbit.RPC.Server.Abstractions.Communication;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Application;
using WebDriver.Application.Commands.ScrollToTop;

namespace WebDriver.Worker.Service.Contracts.ScrollPageTop;

internal sealed record ScrollPageTopContract : IContract;

internal sealed class ScrollPageTopContractHandler(WebDriverApi api)
    : IContractHandler<ScrollPageTopContract>
{
    public async Task<ContractActionResult> Handle(ScrollPageTopContract contract)
    {
        ScrollToTopCommand command = new ScrollToTopCommand();
        Result scrolling = await api.ExecuteCommand(command);

        return scrolling.IsFailure
            ? ContractActionResult.Fail(scrolling.Error.Description)
            : ContractActionResult.Success(true);
    }
}
