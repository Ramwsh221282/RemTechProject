using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Worker.Service.Contracts.BaseImplementations;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

namespace RemTechAvito.Tests.FeaturesTDD.TransportTypesFeature.CustomBehaviors;

public sealed class SetCustomerTypeBehaviour : IWebDriverBehavior
{
    private readonly WebElement _element;

    public SetCustomerTypeBehaviour(WebElement element)
    {
        _element = element;
    }

    public async Task<Result> Execute(IMessagePublisher publisher, CancellationToken ct = default)
    {
        int attempts = 0;
        int maxAttempts = 50;
        while (attempts < maxAttempts)
        {
            if (attempts == maxAttempts - 1)
                return new Error("Unable to set customer type filter");

            ClickOnElementBehavior click = new ClickOnElementBehavior(_element);
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
