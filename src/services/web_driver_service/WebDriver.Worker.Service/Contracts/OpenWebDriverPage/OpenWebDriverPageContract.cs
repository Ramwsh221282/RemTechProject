using System.Text.Json;
using Rabbit.RPC.Server.Abstractions.Communication;
using RemTechCommon.Utils.ResultPattern;

namespace WebDriver.Worker.Service.Contracts.OpenWebDriverPage;

public sealed record OpenWebDriverPageContract(
    string Url,
    string OperationName = nameof(OpenWebDriverPageContract)
) : IContract;

public sealed record OpenWebDriverPageResponse(Result<string> Result) : IContractResponse;

public sealed class OpenWebDriverPageContractHandler : IContractHandler<OpenWebDriverPageContract>
{
    public async Task<string> Handle(OpenWebDriverPageContract contract)
    {
        if (string.IsNullOrWhiteSpace(contract.Url))
        {
            Error error = new Error("Url is not provided.");
            Result<string> result = Result<string>.Failure(error);
            string json = JsonSerializer.Serialize(result);
            return await Task.FromResult(json);
        }

        Result<string> successfullResult = Result<string>.Success(contract.Url);
        OpenWebDriverPageResponse response = new OpenWebDriverPageResponse(successfullResult);
        string jsonResponse = JsonSerializer.Serialize(response);
        return await Task.FromResult(jsonResponse);
    }
}
