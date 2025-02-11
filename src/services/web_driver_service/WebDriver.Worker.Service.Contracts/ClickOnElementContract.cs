using Rabbit.RPC.Client.Abstractions;

namespace WebDriver.Worker.Service.Contracts;

public sealed record ClickOnElementContract(Guid ExistingId) : IContract
{
    public ClickOnElementContract(WebElementResponse element)
        : this(element.ElementId) { }
}

public sealed record ClickOnElementResponse(bool IsClicked);
