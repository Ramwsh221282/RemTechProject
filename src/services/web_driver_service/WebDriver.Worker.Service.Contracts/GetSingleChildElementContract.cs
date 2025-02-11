using Rabbit.RPC.Client.Abstractions;

namespace WebDriver.Worker.Service.Contracts;

public sealed record GetSingleChildElementContract(Guid ParentId, string Path, string Type)
    : IContract
{
    public GetSingleChildElementContract(WebElementResponse element, string Path, string Type)
        : this(element.ElementId, Path, Type) { }
}
