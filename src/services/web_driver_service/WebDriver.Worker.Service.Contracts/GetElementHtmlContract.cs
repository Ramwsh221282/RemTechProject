using Rabbit.RPC.Client.Abstractions;
using WebDriver.Worker.Service.Contracts.Responses;

namespace WebDriver.Worker.Service.Contracts;

public sealed record GetElementHtmlContract(Guid ExistingId) : IContract
{
    public GetElementHtmlContract(WebElementResponse element)
        : this(element.ElementId) { }
}
