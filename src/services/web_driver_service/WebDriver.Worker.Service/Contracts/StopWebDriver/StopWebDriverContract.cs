using Rabbit.RPC.Server.Abstractions.Communication;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Application;
using WebDriver.Application.Commands.StopWebDriver;

namespace WebDriver.Worker.Service.Contracts.StopWebDriver;

internal sealed record StopWebDriverContract : IContract;

internal sealed record StopWebDriverResponse(bool IsStopped);

internal sealed class StopWebDriverContractHandler : IContractHandler<StopWebDriverContract>
{
    private readonly WebDriverApi _api;

    public StopWebDriverContractHandler(WebDriverApi api) => _api = api;

    public async Task<ContractActionResult> Handle(StopWebDriverContract contract)
    {
        StopWebDriverCommand command = new StopWebDriverCommand();
        Result closing = await _api.ExecuteCommand(command);
        if (closing.IsFailure)
            return new ContractActionResult(closing.Error.Description);

        StopWebDriverResponse response = new StopWebDriverResponse(true);
        return new ContractActionResult(response);
    }
}
