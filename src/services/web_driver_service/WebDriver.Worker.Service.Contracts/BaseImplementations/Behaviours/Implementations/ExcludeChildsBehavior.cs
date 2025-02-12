using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;

namespace WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

public sealed class ExcludeChildsBehavior(WebElement element, Predicate<WebElement> Predicate)
    : IWebDriverBehavior
{
    public async Task<Result> Execute(IMessagePublisher publisher, CancellationToken ct = default)
    {
        element.ExcludeChilds(Predicate);
        return await Task.FromResult(Result.Success());
    }
}
