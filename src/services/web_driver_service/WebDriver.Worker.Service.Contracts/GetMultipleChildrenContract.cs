using Rabbit.RPC.Client.Abstractions;

namespace WebDriver.Worker.Service.Contracts;

public sealed record GetMultipleChildrenContract(Guid ParentId, string Path, string Type)
    : IContract
{
    public GetMultipleChildrenContract(WebElementResponse response, string Path, string Type)
        : this(response.ElementId, Path, Type) { }
}

public sealed record GetMultipleChildrenResponse(WebElementResponse[] Results);
