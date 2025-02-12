using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Worker.Service.Contracts.BaseContracts;

namespace WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

public sealed class SendTextNoClearBehavior : IWebDriverBehavior
{
    private readonly WebElement _element;
    private readonly string _text;

    public SendTextNoClearBehavior(WebElement element, string text)
    {
        _element = element;
        _text = text;
    }

    public async Task<Result> Execute(IMessagePublisher publisher, CancellationToken ct = default)
    {
        var result = await publisher.Send(
            new SendTextOnElementNoClearContract(_element.Model.ElementId, _text),
            ct
        );
        return result.ToResult();
    }
}
