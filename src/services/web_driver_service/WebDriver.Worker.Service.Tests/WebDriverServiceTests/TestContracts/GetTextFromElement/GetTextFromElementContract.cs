using Rabbit.RPC.Client.Abstractions;
using WebDriver.Worker.Service.Tests.WebDriverServiceTests.TestContracts.GetSingleElement;

namespace WebDriver.Worker.Service.Tests.WebDriverServiceTests.TestContracts.GetTextFromElement;

internal sealed record GetTextFromElementContract(Guid ExistingId) : IContract
{
    public GetTextFromElementContract(WebElementResponse element)
        : this(element.ElementId) { }
}

internal sealed record GetTextFromElementResponse(string Text);
