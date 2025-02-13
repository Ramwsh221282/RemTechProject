using Rabbit.RPC.Server.Abstractions.Communication;
using WebDriver.Application;
using WebDriver.Application.Commands.ClearPool;

namespace WebDriver.Worker.Service.Contracts.ClearPool;

internal sealed record ClearPoolContract : IContract;

internal sealed class ClearPoolContractHandler(WebDriverApi api)
    : IContractHandler<ClearPoolContract>
{
    public async Task<ContractActionResult> Handle(ClearPoolContract contract)
    {
        ClearPoolCommand command = new();
        await api.ExecuteCommand(command);
        return ContractActionResult.Success(true);
    }
}
