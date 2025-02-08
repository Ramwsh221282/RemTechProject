using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;

namespace WebDriver.Worker.Service.Tests.WebDriverServiceTests.TestContracts.OpenWebDriverPage;

public sealed record OpenWebDriverPageContract(
    string Url,
    string OperationName = nameof(OpenWebDriverPageContract)
) : IContract;

public sealed record OpenWebDriverPageResponse(Result<string> Result) : IContractResponse;
