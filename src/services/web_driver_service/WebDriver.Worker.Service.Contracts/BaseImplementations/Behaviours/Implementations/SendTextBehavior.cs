using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Worker.Service.Contracts.BaseContracts;

namespace WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

public sealed class SendTextBehavior : IWebDriverBehavior
{
    private readonly WebElement _element;
    private readonly string _text;

    public SendTextBehavior(WebElement element, string text)
    {
        _element = element;
        _text = text;
    }

    public async Task<Result> Execute(IMessagePublisher publisher, CancellationToken ct = default)
    {
        var writing = await publisher.Send(
            new SendTextOnElementContract(_element.Model, _text),
            ct
        );
        return writing.ToResult();
    }
}
