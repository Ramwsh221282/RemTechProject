using Rabbit.RPC.Client.Abstractions;

namespace WebDriver.Worker.Service.Contracts.Responses;

public sealed record GetElementAttributeValueContract(Guid ElementId, string AttributeName)
    : IContract
{
    public GetElementAttributeValueContract(WebElementResponse element, string attributeName)
        : this(element.ElementId, attributeName) { }
}
