using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Worker.Service.Contracts.BaseImplementations;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;
using WebDriver.Worker.Service.Contracts.Responses;

namespace RemTechAvito.Tests.FeaturesTDD.TransportTypesFeature.CustomBehaviors;

public sealed class SetLizingNdsFilterBehavior : IWebDriverBehavior
{
    private readonly WebElement _element;
    private const string attributeName = "aria-checked";

    public SetLizingNdsFilterBehavior(WebElement element) => _element = element;

    public async Task<Result> Execute(IMessagePublisher publisher, CancellationToken ct = default)
    {
        int attempt = 0;
        int maxAttempts = 50;
        while (attempt < maxAttempts)
        {
            IWebDriverBehavior click = new ClickOnElementInstant(_element);
            Result clicking = await click.Execute(publisher, ct);
            if (clicking.IsFailure)
            {
                attempt++;
                continue;
            }
            break;
        }
        attempt = 0;
        while (attempt < maxAttempts)
        {
            if (attempt == maxAttempts - 1)
                return new Error("Unable to ensure if nds or lizing is checked");

            var getAttribute = await publisher.Send(
                new GetElementAttributeValueContract(_element.Model.ElementId, attributeName),
                ct
            );
            if (!getAttribute.IsSuccess)
            {
                attempt++;
                continue;
            }

            string value = getAttribute.FromResult<string>();
            if (value != "true")
            {
                attempt++;
                continue;
            }
            break;
        }

        return Result.Success();
    }
}
