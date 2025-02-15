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

        int attempts = 0;
        int maxAttempts = 20;
        while (attempts < maxAttempts)
        {
            try
            {
                string url = driver.Url;
                return url;
            }
            catch
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                attempts++;
            }
        }

        return new Error("Unable to validate opened page is the same as requested page");
    }
}
