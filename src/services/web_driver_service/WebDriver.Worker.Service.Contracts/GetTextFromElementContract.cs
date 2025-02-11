using Rabbit.RPC.Client.Abstractions;

namespace WebDriver.Worker.Service.Contracts;

public sealed record GetTextFromElementContract(Guid ExistingId) : IContract
{
    public GetTextFromElementContract(WebElementResponse element)
        : this(element.ElementId) { }
}

public sealed record GetTextFromElementResponse(string Text);
