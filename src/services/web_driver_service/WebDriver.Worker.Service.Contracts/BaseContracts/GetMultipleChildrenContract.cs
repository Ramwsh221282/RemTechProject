using Rabbit.RPC.Client.Abstractions;
using WebDriver.Worker.Service.Contracts.Responses;

namespace WebDriver.Worker.Service.Contracts.BaseContracts;

public sealed record GetMultipleChildrenContract(Guid ParentId, string Path, string Type)
    : IContract
{
    public GetMultipleChildrenContract(WebElementResponse response, string Path, string Type)
        : this(response.ElementId, Path, Type) { }
}
