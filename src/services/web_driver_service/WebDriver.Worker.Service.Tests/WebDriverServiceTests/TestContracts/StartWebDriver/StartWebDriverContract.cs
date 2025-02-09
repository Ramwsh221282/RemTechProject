using Rabbit.RPC.Client.Abstractions;

namespace WebDriver.Worker.Service.Tests.WebDriverServiceTests.TestContracts.StartWebDriver;

public record StartWebDriverContract : IContract;

public record StartWebDriverContractResponse(bool IsStarted);
