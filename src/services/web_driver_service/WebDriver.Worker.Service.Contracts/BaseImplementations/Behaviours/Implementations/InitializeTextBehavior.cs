using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Worker.Service.Contracts.BaseContracts;
using WebDriver.Worker.Service.Contracts.Utils;

namespace WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

public class InitializeTextBehavior(WebElement element) : IWebDriverBehavior
{
    public virtual async Task<Result> Execute(
        IMessagePublisher publisher,
        CancellationToken ct = default
    )
    {
        var request = await publisher.Send(
            new GetTextFromElementContract(element.Model.ElementId),
            ct
        );

        Result result = request.ToResult();
        if (result.IsFailure)
            return result;

        string text = request.FromResult<string>();
        element.SetText(text);
        return Result.Success();
    }
}

public sealed class InitializeTextRepeatable(WebElement element, byte maxAttempts)
    : InitializeTextBehavior(element),
        IRetriable
{
    public byte MaxAttempts { get; } = maxAttempts;

    public override async Task<Result> Execute(
        IMessagePublisher publisher,
        CancellationToken ct = default
    )
    {
        int attempts = 0;
        while (attempts < MaxAttempts)
        {
            var request = await base.Execute(publisher, ct);

            if (request.IsSuccess)
                return request;

            attempts++;
            await Task.Delay(TimeSpan.FromSeconds(1));
        }

        return new Error($"Cannot initialize text. Attempts: {MaxAttempts}");
    }
}
