using OpenQA.Selenium;
using RemTechCommon.Utils.ResultPattern;

namespace WebDriver.Core.Models.InteractionStrategies;

internal sealed class ExtractHtmlInteraction : IInteractionStrategy<string>
{
    public async Task<Result<string>> Perform(WebDriverInstance instance)
    {
        Result<IWebDriver> request = instance.GetRunningDriver();
        if (request.IsFailure)
            return request.Error;

        IWebDriver driver = request.Value;
        string html = driver.PageSource;
        return await Task.FromResult(html);
    }
}
