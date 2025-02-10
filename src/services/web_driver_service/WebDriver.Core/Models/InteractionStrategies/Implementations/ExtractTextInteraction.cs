using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using RemTechCommon.Utils.ResultPattern;

namespace WebDriver.Core.Models.InteractionStrategies.Implementations;

internal sealed class ExtractTextInteraction : IInteractionStrategy<string>
{
    private const string Script = "return arguments[0].innerText;";
    private readonly Guid _elementId;

    public ExtractTextInteraction(Guid elementId) => _elementId = elementId;

    public async Task<Result<string>> Perform(WebDriverInstance instance)
    {
        Result<IWebDriver> request = instance.GetRunningDriver();
        if (request.IsFailure)
            return await Task.FromResult(request.Error);

        Result<WebElementObject> element = instance.GetFromPool(_elementId);
        if (element.IsFailure)
            return element.Error;

        IWebDriver driver = request.Value;

        string text = driver.ExecuteJavaScript<string>(Script, element.Value.Model)!;
        return text;
    }
}
