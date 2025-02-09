using Rabbit.RPC.Client.Abstractions;
using WebDriver.Worker.Service.Tests.WebDriverServiceTests.TestContracts.GetSingleElement;

namespace WebDriver.Worker.Service.Tests.WebDriverServiceTests.TestContracts.GetMultipleChildren;

public sealed record GetMultipleChildrenContract(Guid ParentId, string Path, string Type)
    : IContract
{
    public GetMultipleChildrenContract(WebElementResponse response, string Path, string Type)
        : this(response.ElementId, Path, Type) { }
}

public sealed record GetMultipleChildrenResponse(WebElementResponse[] Results);
