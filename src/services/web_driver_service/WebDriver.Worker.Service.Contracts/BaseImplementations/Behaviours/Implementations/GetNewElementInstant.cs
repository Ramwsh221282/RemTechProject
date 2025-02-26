using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Worker.Service.Contracts.BaseContracts;
using WebDriver.Worker.Service.Contracts.Responses;
using WebDriver.Worker.Service.Contracts.Utils;

namespace WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

public abstract class GetSingleElementBasic(
    WebElementPool pool,
    string path,
    string pathType,
    string name
) : IWebDriverBehavior
{
    public virtual async Task<Result> Execute(
        IMessagePublisher publisher,
        CancellationToken ct = default
    )
    {
        var request = await publisher.Send(new GetSingleElementContract(path, pathType), ct);
        var response = request.FromResult<WebElementResponse>();
        var result = request.ToResult();

        if (result.IsFailure)
            return result;

        WebElement element = new(response, name);
        pool.AddElement(element);
        return result;
    }
}

public sealed class GetNewElementInstant(
    WebElementPool pool,
    string path,
    string pathType,
    string name
) : GetSingleElementBasic(pool, path, pathType, name) { }

public sealed class GetNewElementRetriable(
    WebElementPool pool,
    string path,
    string pathType,
    string name,
    byte maxAttempts
) : GetSingleElementBasic(pool, path, pathType, name), IRetriable
{
    public byte MaxAttempts { get; } = maxAttempts;

    public override async Task<Result> Execute(
        IMessagePublisher publisher,
        CancellationToken ct = default
    )
    {
        var attempts = 0;
        while (attempts < MaxAttempts)
        {
            var execution = await base.Execute(publisher, ct);

            if (execution.IsSuccess)
                return execution;

            await Task.Delay(TimeSpan.FromSeconds(1), ct);
            attempts++;
        }

        return new Error($"Getting element resulted in fail. Attempts: {MaxAttempts}");
    }
}
