using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Core.Models.SearchStrategies;

namespace WebDriver.Core.Models.InteractionStrategies.Implementations;

internal sealed class ScrollToBottomInteraction : BaseSearchElementStrategy, IInteractionStrategy
{
    private bool _isScrolled;

    private const string GetCurrentHeightScript =
        "return Math.max(document.documentElement.scrollHeight, document.body.scrollHeight);";

    private const string ScrollToBottomScript =
        "window.scrollTo(0, document.documentElement.scrollHeight);";

    public async Task<Result> Perform(WebDriverInstance instance)
    {
        Result<IWebDriver> request = instance.GetRunningDriver();
        if (request.IsFailure)
            return request;

        IWebDriver driver = request.Value;

        int attempts = 0;
        while (ReachedMaxAttempts(ref attempts))
        {
            try
            {
                while (!_isScrolled)
                {
                    driver.ExecuteJavaScript(ScrollToBottomScript);
                    long initialHeight = driver.ExecuteJavaScript<long>(GetCurrentHeightScript);
                    long currentHeight = driver.ExecuteJavaScript<long>(GetCurrentHeightScript);
                    if (!IsEndOfPage(ref initialHeight, ref currentHeight))
                        continue;

                    _isScrolled = true;
                }
                break;
            }
            catch
            {
                attempts++;
                await Wait();
            }
        }

        if (ReachedMaxAttempts(ref attempts) && !_isScrolled)
            return new Error("Cannot scroll to bottom.");

        return await Task.FromResult(Result.Success());
    }

    private static bool IsEndOfPage(ref long initialHeight, ref long currentHeight) =>
        currentHeight == initialHeight;
}
