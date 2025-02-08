using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;

namespace WebDriver.Worker.Service.Tests.WebDriverServiceTests.TestContracts.StopWebDriver;

public sealed record StopWebDriverContract(string OperationName = nameof(StopWebDriverContract))
    : IContract;

public sealed record StopWebDriverContractResponse(Result Result);
