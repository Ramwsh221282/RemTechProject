using Rabbit.RPC.Client.Abstractions;
using WebDriver.Worker.Service.Contracts.Responses;

namespace WebDriver.Worker.Service.Contracts.BaseContracts;

public sealed record ClearTextContract(Guid ExistingId) : IContract
{
    public ClearTextContract(WebElementResponse element)
        : this(element.ElementId) { }
}
