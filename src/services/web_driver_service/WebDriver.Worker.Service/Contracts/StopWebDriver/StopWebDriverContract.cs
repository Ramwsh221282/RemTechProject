using Rabbit.RPC.Server.Abstractions.Communication;

namespace WebDriver.Worker.Service.Contracts.StopWebDriver;

internal sealed record StopWebDriverContract : IContract;

internal sealed record StopWebDriverContractResponse(bool IsStopped);

internal sealed class StopWebDriverContractHandler : IContractHandler<StopWebDriverContract>
{
    public async Task<ContractActionResult> Handle(StopWebDriverContract contract)
    {
        StopWebDriverContractResponse response = new(true);
        ContractActionResult result = new(response);
        return await Task.FromResult(result);
    }
}
