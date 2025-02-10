using OpenQA.Selenium;
using RemTechCommon.Utils.ResultPattern;

namespace WebDriver.Core.Models.InteractionStrategies.Implementations;

internal sealed class ClickOnElementStrategy : IInteractionStrategy
{
    private readonly Guid _elementId;

    public ClickOnElementStrategy(Guid elementId) => _elementId = elementId;

    public async Task<Result> Perform(WebDriverInstance instance)
    {
        Result<IWebDriver> request = instance.GetRunningDriver();
        if (request.IsFailure)
            return request.Error;

        Result<WebElementObject> element = instance.GetFromPool(_elementId);
        if (element.IsFailure)
            return element.Error;

        try
        {
            element.Value.Model.Click();
            return await Task.FromResult(Result.Success());
        }
        catch
        {
            Error error = new Error(
                $"Element with path: {element.Value.ElementPath} type: {element.Value.ElementPathType} id: {element.Value.ElementId} is not clickable"
            );
            return await Task.FromResult(error);
        }
    }
}
