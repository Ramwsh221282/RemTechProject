using Rabbit.RPC.Client.Abstractions;
using WebDriver.Worker.Service.Tests.WebDriverServiceTests.TestContracts.GetSingleElement;

namespace WebDriver.Worker.Service.Tests.WebDriverServiceTests.TestContracts.ClickOnElement;

public sealed record ClickOnElementContract(Guid ExistingId) : IContract
{
    public ClickOnElementContract(WebElementResponse element)
        : this(element.ElementId) { }
}

internal sealed record ClickOnElementResponse(bool IsClicked);
