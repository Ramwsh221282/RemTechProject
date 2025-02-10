using OpenQA.Selenium;
using RemTechCommon.Utils.ResultPattern;

namespace WebDriver.Core.Models.InteractionStrategies.Implementations;

internal sealed class OpenPageInteraction : IInteractionStrategy<string>
{
    private readonly string _webPageUrl;

    public OpenPageInteraction(string webPageUrl) => _webPageUrl = webPageUrl;

    public async Task<Result<string>> Perform(WebDriverInstance instance)
    {
        Result<IWebDriver> request = instance.GetRunningDriver();
        if (request.IsFailure)
            return await Task.FromResult(request.Error);

        IWebDriver driver = request.Value;
        await driver.Navigate().GoToUrlAsync(_webPageUrl);
        string url = driver.Url;

        return url == _webPageUrl
            ? await Task.FromResult(url)
            : await Task.FromResult(
                new Error("Opened page url is different from command page url")
            );
    }
}
