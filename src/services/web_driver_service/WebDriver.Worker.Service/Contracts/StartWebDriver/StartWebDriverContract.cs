using Rabbit.RPC.Server.Abstractions.Communication;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Application;
using WebDriver.Application.Commands.StartWebDriver;
using WebDriver.Application.DTO;

namespace WebDriver.Worker.Service.Contracts.StartWebDriver;

internal record StartWebDriverContract(string LoadStrategy) : IContract;

internal sealed class StartWebDriverContractHandler(WebDriverApi api)
    : IContractHandler<StartWebDriverContract>
{
    public async Task<ContractActionResult> Handle(StartWebDriverContract contract)
    {
        DriverStartDataDTO data = new(contract.LoadStrategy);
        StartWebDriverCommand command = new(data);

        Result starting = await api.ExecuteCommand(command);
        return starting.IsFailure
            ? ContractActionResult.Fail(starting.Error.Description)
            : ContractActionResult.Success(true);
    }
}
