using Rabbit.RPC.Server.Abstractions.Communication;

namespace WebDriver.Worker.Service.Contracts.OpenWebDriverPage;

internal sealed record OpenWebDriverPageContract(string Url) : IContract;

internal sealed record OpenWebDriverPageResponse(string OpenedUrl);

internal sealed class OpenWebDriverPageContractHandler : IContractHandler<OpenWebDriverPageContract>
{
    public async Task<ContractActionResult> Handle(OpenWebDriverPageContract contract)
    {
        if (string.IsNullOrWhiteSpace(contract.Url))
            return new("Url is not provided.");

        return await Task.FromResult(
            new ContractActionResult(new OpenWebDriverPageResponse(contract.Url))
        );
    }
}
