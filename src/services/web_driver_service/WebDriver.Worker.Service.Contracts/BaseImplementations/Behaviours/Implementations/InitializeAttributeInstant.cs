using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Worker.Service.Contracts.Responses;
using WebDriver.Worker.Service.Contracts.Utils;

namespace WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

public abstract class BaseInitializeAttributeBehavior(WebElement element, string attribute)
    : IWebDriverBehavior
{
    public virtual async Task<Result> Execute(
        IMessagePublisher publisher,
        CancellationToken ct = default
    )
    {
        var request = await publisher.Send(
            new GetElementAttributeValueContract(element.Model.ElementId, attribute),
            ct
        );

        Result result = request.ToResult();
        if (result.IsFailure)
            return result;

        string value = request.FromResult<string>();
        if (string.IsNullOrWhiteSpace(value))
            return new Error("Attribute value was empty");

        element.SetAttribute(attribute, value);
        return Result.Success();
    }
}

public sealed class InitializeAttributeInstant(WebElement element, string attribute)
    : BaseInitializeAttributeBehavior(element, attribute);

public sealed class InitializeAttributeRepeatable(
    WebElement element,
    string attribute,
    byte maxAttempts
) : BaseInitializeAttributeBehavior(element, attribute), IRetriable
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
        return new Error($"Cannot initialize attribute. Attempts tried: {MaxAttempts}");
    }
}
