using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Worker.Service.Contracts.BaseContracts;
using WebDriver.Worker.Service.Contracts.BaseImplementations;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours;
using WebDriver.Worker.Service.Contracts.Responses;

namespace RemTechAvito.Tests.FeaturesTDD.TransportTypesFeature.CustomBehaviors;

public sealed class SetAvitoTransportTypeFiltersBehavior(WebElement Element, string[] filters)
    : IWebDriverBehavior
{
    public async Task<Result> Execute(IMessagePublisher publisher, CancellationToken ct = default)
    {
        Element.ExcludeChilds(element => filters.All(param => element.Text != param));
        await Element.ExecuteForChilds(
            publisher,
            element => new ClickOnCheckBoxesWithEnsureClicked(element),
            ct
        );
        return await Task.FromResult(Result.Success());
    }

    private sealed class ClickOnCheckBoxesWithEnsureClicked(WebElement element) : IWebDriverBehavior
    {
        private const string AttributeName = "aria-checked";
        private const string AttributeValue = "true";

        public async Task<Result> Execute(
            IMessagePublisher publisher,
            CancellationToken ct = default
        )
        {
            bool isClicked = false;
            while (!isClicked)
            {
                ContractActionResult click = await publisher.Send(
                    new ClickOnElementContract(element.Model),
                    ct
                );
                isClicked = click.IsSuccess;
            }

            bool isAttributeSet = false;
            while (!isAttributeSet)
            {
                ContractActionResult getAttribute = await publisher.Send(
                    new GetElementAttributeValueContract(element.Model, AttributeName),
                    ct
                );
                string attribute = getAttribute.FromResult<string>();
                if (attribute != AttributeValue)
                    continue;
                element.SetAttribute(AttributeName, AttributeValue);
                isAttributeSet = true;
            }

            return Result.Success();
        }
    }
}
