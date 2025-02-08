using System.Text.Json;
using Rabbit.RPC.Server.Abstractions.Communication;
using RemTechCommon.Utils.ResultPattern;

namespace WebDriver.Worker.Service.Contracts.StopWebDriver;

public sealed record StopWebDriverContract(string OperationName = nameof(StopWebDriverContract))
    : IContract;

public sealed record StopWebDriverContractResponse(Result Result);

public sealed class StopWebDriverContractHandler : IContractHandler<StopWebDriverContract>
{
    public async Task<string> Handle(StopWebDriverContract contract)
    {
        Result result = Result.Success();
        string json = JsonSerializer.Serialize(result);
        return await Task.FromResult(json);
    }
}
