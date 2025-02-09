using Rabbit.RPC.Client.Abstractions;

namespace WebDriver.Worker.Service.Tests.WebDriverServiceTests.TestContracts.StopWebDriver;

public sealed record StopWebDriverContract : IContract;

public sealed record StopWebDriverContractResponse(bool IsStopped);
