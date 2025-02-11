using Rabbit.RPC.Client.Abstractions;
using WebDriver.Worker.Service.Contracts.Responses;

namespace WebDriver.Worker.Service.Contracts;

public sealed record GetTextFromElementContract(Guid ExistingId) : IContract
{
    public GetTextFromElementContract(WebElementResponse element)
        : this(element.ElementId) { }
}
