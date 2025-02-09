using Rabbit.RPC.Server.Abstractions.Communication;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Core.Commands.ScrollToTop;
using WebDriver.Core.Core;
using WebDriver.Worker.Service.Contracts.ScrollPageDown;

namespace WebDriver.Worker.Service.Contracts.ScrollPageTop;

internal sealed record ScrollPageTopContract : IContract;

internal sealed record ScrollPageTopResponse(bool IsScrolled);

internal sealed class ScrollPageTopContractHandler : IContractHandler<ScrollPageTopContract>
{
    private readonly WebDriverApi _api;

    public ScrollPageTopContractHandler(WebDriverApi api) => _api = api;

    public async Task<ContractActionResult> Handle(ScrollPageTopContract contract)
    {
        ScrollToTopCommand command = new ScrollToTopCommand();
        Result scrolling = await _api.ExecuteCommand(command);
        if (scrolling.IsFailure)
            return new ContractActionResult(scrolling.Error.Description);

        ScrollPageDownContractResponse response = new(true);
        ContractActionResult result = new(response);
        return result;
    }
}
