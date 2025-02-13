using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Worker.Service.Contracts.BaseImplementations;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

namespace RemTechAvito.Tests.FeaturesTDD.TransportTypesFeature.CustomBehaviors;

public sealed class SetFiltersBehavior : IWebDriverBehavior
{
    private readonly WebElement _element;

    public SetFiltersBehavior(WebElement element)
    {
        _element = element;
    }

    public async Task<Result> Execute(IMessagePublisher publisher, CancellationToken ct = default)
    {
        int attempts = 0;
        int maxAttempts = 50;

        while (true)
        {
            if (attempts == maxAttempts - 1)
                return new Error("Unable to click on submit filter button");

            IWebDriverBehavior click = new ClickOnElementInstant(_element);
            Result clicking = await click.Execute(publisher, ct);

            if (clicking.IsFailure)
            {
                attempts++;
                continue;
            }

            break;
        }

        return Result.Success();
    }
}
