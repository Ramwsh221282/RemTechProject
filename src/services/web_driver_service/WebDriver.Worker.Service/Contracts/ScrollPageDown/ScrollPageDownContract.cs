using Rabbit.RPC.Server.Abstractions.Communication;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Core.Commands.ScrollToDown;
using WebDriver.Core.Core;

namespace WebDriver.Worker.Service.Contracts.ScrollPageDown;

internal sealed record ScrollPageDownContract : IContract;

internal sealed record ScrollPageDownContractResponse(bool IsScrolled);

internal sealed class ScrollPageDownContractHandler : IContractHandler<ScrollPageDownContract>
{
    private readonly WebDriverApi _api;

    public ScrollPageDownContractHandler(WebDriverApi api) => _api = api;

    public async Task<ContractActionResult> Handle(ScrollPageDownContract contract)
    {
        ScrollToDownCommand command = new ScrollToDownCommand();
        Result scrolling = await _api.ExecuteCommand(command);
        if (scrolling.IsFailure)
            return new ContractActionResult(scrolling.Error);

        ScrollPageDownContractResponse response = new(true);
        ContractActionResult result = new(response);
        return result;
    }
}
