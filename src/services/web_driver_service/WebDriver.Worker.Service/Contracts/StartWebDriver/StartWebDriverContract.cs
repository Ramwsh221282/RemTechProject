using System.Text.Json;
using Rabbit.RPC.Server.Abstractions.Communication;
using RemTechCommon.Utils.ResultPattern;

namespace WebDriver.Worker.Service.Contracts.StartWebDriver;

public record StartWebDriverContract(string OperationName = nameof(StartWebDriverContract))
    : IContract;

public record StopWebDriverResponse(Result Result) : IContractResponse;

public sealed class StartWebDriverContractHandler : IContractHandler<StartWebDriverContract>
{
    public async Task<string> Handle(StartWebDriverContract contract)
    {
        Result result = Result.Success();
        string json = JsonSerializer.Serialize(result);
        return await Task.FromResult(json);
    }
}
