using Rabbit.RPC.Client.Abstractions;

namespace WebDriver.Worker.Service.Tests.WebDriverServiceTests.TestContracts.ScrollPageDown;

public record ScrollPageDownContract : IContract;

public sealed record ScrollPageDownContractResponse(bool IsScrolled);
