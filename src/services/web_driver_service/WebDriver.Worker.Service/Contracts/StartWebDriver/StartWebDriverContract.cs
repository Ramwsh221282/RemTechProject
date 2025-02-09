using Rabbit.RPC.Server.Abstractions.Communication;

namespace WebDriver.Worker.Service.Contracts.StartWebDriver;

internal record StartWebDriverContract : IContract;

internal record StopWebDriverResponse(bool IsStarted);

internal sealed class StartWebDriverContractHandler : IContractHandler<StartWebDriverContract>
{
    public async Task<ContractActionResult> Handle(StartWebDriverContract contract)
    {
        StopWebDriverResponse response = new(true);
        ContractActionResult result = new(response);
        return await Task.FromResult(result);
    }
}
