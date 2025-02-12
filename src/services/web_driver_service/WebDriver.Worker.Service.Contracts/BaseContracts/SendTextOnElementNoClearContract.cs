using Rabbit.RPC.Client.Abstractions;
using WebDriver.Worker.Service.Contracts.Responses;

namespace WebDriver.Worker.Service.Contracts.BaseContracts;

public sealed record SendTextOnElementNoClearContract(Guid ElementId, string Text) : IContract
{
    public SendTextOnElementNoClearContract(WebElementResponse element, string text)
        : this(element.ElementId, text) { }
}
