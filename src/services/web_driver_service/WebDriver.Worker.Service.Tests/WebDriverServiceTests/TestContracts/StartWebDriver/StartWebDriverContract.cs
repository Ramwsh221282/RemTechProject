using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;

namespace WebDriver.Worker.Service.Tests.WebDriverServiceTests.TestContracts.StartWebDriver;

public record StartWebDriverContract(string OperationName = nameof(StartWebDriverContract))
    : IContract;

public record StartWebDriverContractResponse(Result Result) : IContractResponse;
