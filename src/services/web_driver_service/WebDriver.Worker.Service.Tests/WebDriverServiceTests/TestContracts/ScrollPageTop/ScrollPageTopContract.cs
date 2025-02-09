using Rabbit.RPC.Client.Abstractions;

namespace WebDriver.Worker.Service.Tests.WebDriverServiceTests.TestContracts.ScrollPageTop;

internal sealed record ScrollPageTopContract : IContract;

internal sealed record ScrollPageTopResponse(bool IsScrolled);
