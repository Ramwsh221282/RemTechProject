using Rabbit.RPC.Server.Abstractions.Communication;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Application;
using WebDriver.Application.Commands.StopWebDriver;

namespace WebDriver.Worker.Service.Contracts.StopWebDriver;

internal sealed record StopWebDriverContract : IContract;

internal sealed class StopWebDriverContractHandler(WebDriverApi api)
    : IContractHandler<StopWebDriverContract>
{
    public async Task<ContractActionResult> Handle(StopWebDriverContract contract)
    {
        StopWebDriverCommand command = new StopWebDriverCommand();
        Result closing = await api.ExecuteCommand(command);

        return closing.IsFailure
            ? ContractActionResult.Fail(closing.Error.Description)
            : ContractActionResult.Success(true);
    }
}
