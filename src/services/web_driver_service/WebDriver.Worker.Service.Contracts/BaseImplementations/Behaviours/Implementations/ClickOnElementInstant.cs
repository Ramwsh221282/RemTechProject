using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Worker.Service.Contracts.BaseContracts;
using WebDriver.Worker.Service.Contracts.Utils;

namespace WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

public abstract class ClickOnElementBehavior(WebElement element) : IWebDriverBehavior
{
    public virtual async Task<Result> Execute(
        IMessagePublisher publisher,
        CancellationToken ct = default
    )
    {
        var clicking = await publisher.Send(new ClickOnElementContract(element.Model), ct);
        return clicking.ToResult();
    }
}

public sealed class ClickOnElementRetriable(WebElement element, byte maxAttempts)
    : ClickOnElementBehavior(element),
        IRetriable
{
    public byte MaxAttempts { get; } = maxAttempts;

    public override async Task<Result> Execute(
        IMessagePublisher publisher,
        CancellationToken ct = default
    )
    {
        byte current = 0;
        while (current < MaxAttempts)
        {
            Result execution = await base.Execute(publisher, ct);
            if (execution.IsSuccess)
                return execution;

            await Task.Delay(TimeSpan.FromSeconds(1));
            current++;
        }

        return new Error($"Click does not resulsted in success. Attempts: {MaxAttempts}");
    }
}

public sealed class ClickOnElementInstant(WebElement element) : ClickOnElementBehavior(element);
