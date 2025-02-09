using Rabbit.RPC.Client.Abstractions;

namespace WebDriver.Worker.Service.Tests.WebDriverServiceTests.TestContracts.OpenWebDriverPage;

public sealed record OpenWebDriverPageContract(string Url) : IContract;

public sealed record OpenWebDriverPageResponse(string OpenedUrl);
