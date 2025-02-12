using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Worker.Service.Contracts.BaseContracts;
using WebDriver.Worker.Service.Contracts.BaseImplementations;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;
using WebDriver.Worker.Service.Contracts.Responses;

namespace RemTechAvito.Tests.FeaturesTDD.TransportTypesFeature.CustomBehaviors;

public class SetAvitoMarkFiltersBehavior : IWebDriverBehavior
{
    private readonly WebElement _element;

    public SetAvitoMarkFiltersBehavior(WebElement element)
    {
        _element = element;
    }

    public async Task<Result> Execute(IMessagePublisher publisher, CancellationToken ct = default)
    {
        IWebDriverBehavior behavior = new Implementation(_element);
        return await behavior.Execute(publisher, ct);
    }

    private sealed class Implementation : IWebDriverBehavior
    {
        private const string markInputXPath =
            ".//input[@data-marker='params[111022]/multiselect-outline-input/input']";
        private const string markCheckBoxesContainerXPath =
            ".//div[@data-marker='params[111022]/list']";
        private const string markCheckBoxXPath = ".//label[@role='checkbox']";
        private const string pathType = "xpath";
        private const string checkBoxClickedAttribute = "aria-checked";

        private readonly WebElement _element;

        public Implementation(WebElement element) => _element = element;

        public async Task<Result> Execute(
            IMessagePublisher publisher,
            CancellationToken ct = default
        )
        {
            var markInput = await GetMarkInput(publisher, ct);
            if (markInput.IsFailure)
                return markInput;

            var clickMarkInput = await PerformClick(markInput, publisher, ct);
            if (clickMarkInput.IsFailure)
                return clickMarkInput;

            var markInputElement = new WebElement(markInput, "temporary");
            var writeFilterText = await WriteFilterText(markInputElement, publisher, ct);
            if (writeFilterText.IsFailure)
                return writeFilterText;

            int attempts = 0;
            int maxAttempts = 50;

            while (true)
            {
                if (attempts == maxAttempts)
                    return new Error("Can't write mark filters");

                var checkBoxContainer = await GetCheckBoxContainer(publisher, ct);
                if (checkBoxContainer.IsFailure)
                {
                    attempts++;
                    continue;
                }

                var checkboxes = await GetCheckBoxElements(checkBoxContainer, publisher, ct);
                if (checkboxes.IsFailure)
                {
                    attempts++;
                    continue;
                }

                int checkBoxesLength = checkboxes.Value.Length;
                if (checkBoxesLength > 1)
                {
                    attempts++;
                    continue;
                }

                WebElementResponse checkBox = checkboxes.Value[0];
                while (true)
                {
                    Result click = await PerformClick(checkBox, publisher, ct);
                    if (click.IsSuccess)
                        break;
                }

                while (true)
                {
                    Result<bool> isChecked = await EnsureCheckBoxClicked(checkBox, publisher, ct);
                    if (isChecked.IsFailure)
                        return isChecked;

                    if (isChecked.Value)
                        break;
                }

                var checkText = await publisher.Send(new GetTextFromElementContract(checkBox), ct);
                if (!checkText.IsSuccess)
                    return new Error($"Cannot ensure text of filter parameter {_element.Text}");

                string writtenText = checkText.FromResult<string>();
                if (writtenText != _element.Text)
                    return new Error($"Text is not matching filter parameter: {_element.Text}");

                ClearTextBehavior clearingText = new ClearTextBehavior(markInputElement);
                return await clearingText.Execute(publisher, ct);
            }
        }

        private async Task<Result<WebElementResponse>> GetMarkInput(
            IMessagePublisher publisher,
            CancellationToken ct = default
        )
        {
            var getMarkInput = await publisher.Send(
                new GetSingleElementContract(markInputXPath, pathType),
                ct
            );

            return getMarkInput.IsSuccess
                ? getMarkInput.FromResult<WebElementResponse>()
                : new Error(getMarkInput.Error);
        }

        private async Task<Result> PerformClick(
            WebElementResponse markInput,
            IMessagePublisher publisher,
            CancellationToken ct = default
        )
        {
            var clickMarkInput = await publisher.Send(new ClickOnElementContract(markInput), ct);
            return clickMarkInput.IsSuccess ? Result.Success() : new Error(clickMarkInput.Error);
        }

        private async Task<Result> WriteFilterText(
            WebElement markInput,
            IMessagePublisher publisher,
            CancellationToken ct = default
        )
        {
            foreach (var character in _element.Text)
            {
                IWebDriverBehavior behavior = new SendTextNoClearBehavior(
                    markInput,
                    character.ToString()
                );
                var writing = await behavior.Execute(publisher, ct);
                if (writing.IsFailure)
                    return writing;
            }

            return Result.Success();
        }

        private async Task<Result<WebElementResponse>> GetCheckBoxContainer(
            IMessagePublisher publisher,
            CancellationToken ct = default
        )
        {
            var request = await publisher.Send(
                new GetSingleElementContract(markCheckBoxesContainerXPath, pathType),
                ct
            );

            return request.IsSuccess
                ? request.FromResult<WebElementResponse>()
                : new Error(request.Error);
        }

        private async Task<Result<WebElementResponse[]>> GetCheckBoxElements(
            WebElementResponse container,
            IMessagePublisher publisher,
            CancellationToken ct = default
        )
        {
            var request = await publisher.Send(
                new GetMultipleChildrenContract(container, markCheckBoxXPath, pathType),
                ct
            );

            return request.IsSuccess
                ? request.FromResult<WebElementResponse[]>()
                : new Error(request.Error);
        }

        private async Task<Result<bool>> EnsureCheckBoxClicked(
            WebElementResponse checkbox,
            IMessagePublisher publisher,
            CancellationToken ct = default
        )
        {
            var request = await publisher.Send(
                new GetElementAttributeValueContract(checkbox, checkBoxClickedAttribute),
                ct
            );

            return request.IsSuccess
                ? request.FromResult<string>() == "true"
                : new Error(request.Error);
        }
    }
}
