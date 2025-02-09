using Rabbit.RPC.Client.Abstractions;
using WebDriver.Worker.Service.Tests.WebDriverServiceTests.TestContracts.GetSingleElement;

namespace WebDriver.Worker.Service.Tests.WebDriverServiceTests.TestContracts.GetSingleChildElement;

public sealed record GetSingleChildElementContract(Guid ParentId, string Path, string Type)
    : IContract
{
    public GetSingleChildElementContract(WebElementResponse element, string Path, string Type)
        : this(element.ElementId, Path, Type) { }
}
