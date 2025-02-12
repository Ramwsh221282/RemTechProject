using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours;

namespace WebDriver.Worker.Service.Contracts.BaseImplementations;

public sealed class WebDriverSession(IMessagePublisher publisher) : IDisposable
{
    public void Dispose()
    {
        if (publisher is IDisposable disposable)
            disposable.Dispose();
    }

    public async Task<Result> ExecuteBehavior(
        IWebDriverBehavior behavior,
        CancellationToken ct = default
    ) => await behavior.Execute(publisher, ct);
}
