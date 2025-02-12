using Rabbit.RPC.Client.Abstractions;
using WebDriver.Worker.Service.Contracts.Responses;

namespace WebDriver.Worker.Service.Contracts.BaseContracts;

public sealed record GetSingleChildElementContract(Guid ParentId, string Path, string Type)
    : IContract
{
    public GetSingleChildElementContract(WebElementResponse element, string Path, string Type)
        : this(element.ElementId, Path, Type) { }
}
