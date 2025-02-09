using Rabbit.RPC.Server.Abstractions.Communication;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Core.Commands.StartWebDriver;
using WebDriver.Core.Core;

namespace WebDriver.Worker.Service.Contracts.StartWebDriver;

internal record StartWebDriverContract : IContract;

internal record StartWebDriverResponse(bool IsStarted);

internal sealed class StartWebDriverContractHandler : IContractHandler<StartWebDriverContract>
{
    private readonly WebDriverApi _api;

    public StartWebDriverContractHandler(WebDriverApi api) => _api = api;

    public async Task<ContractActionResult> Handle(StartWebDriverContract contract)
    {
        StartWebDriverCommand command = new StartWebDriverCommand();
        Result starting = await _api.ExecuteCommand(command);
        if (starting.IsFailure)
            return new ContractActionResult(starting.Error.Description);

        StartWebDriverResponse response = new StartWebDriverResponse(true);
        return new ContractActionResult(response);
    }
}
