using Rabbit.RPC.Client.Abstractions;
using WebDriver.Worker.Service.Contracts.Responses;

namespace WebDriver.Worker.Service.Contracts;

public sealed record SendTextOnElementContract(Guid ElementId, string Text) : IContract
{
    public SendTextOnElementContract(WebElementResponse element, string text)
        : this(element.ElementId, text) { }
}
