using Rabbit.RPC.Client.Abstractions;
using WebDriver.Worker.Service.Contracts.Responses;

namespace WebDriver.Worker.Service.Contracts;

public sealed record ClickOnElementContract(Guid ExistingId) : IContract
{
    public ClickOnElementContract(WebElementResponse element)
        : this(element.ElementId) { }
}
