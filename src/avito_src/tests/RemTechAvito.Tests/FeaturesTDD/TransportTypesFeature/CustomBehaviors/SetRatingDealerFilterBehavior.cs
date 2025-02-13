using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Worker.Service.Contracts.BaseImplementations;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

namespace RemTechAvito.Tests.FeaturesTDD.TransportTypesFeature.CustomBehaviors;

public sealed class SetRatingDealerFilterBehavior : IWebDriverBehavior
{
    private readonly WebElement _element;

    public SetRatingDealerFilterBehavior(WebElement element)
    {
        _element = element;
    }

    public async Task<Result> Execute(IMessagePublisher publisher, CancellationToken ct = default)
    {
        int attempts = 0;
        int limit = 50;
        while (true)
        {
            if (attempts == limit - 1)
                return new Error("Cannot set rating or dealer filter");

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
