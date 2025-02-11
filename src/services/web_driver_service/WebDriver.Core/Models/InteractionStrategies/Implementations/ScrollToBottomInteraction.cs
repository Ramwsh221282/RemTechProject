using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using RemTechCommon.Utils.ResultPattern;

namespace WebDriver.Core.Models.InteractionStrategies.Implementations;

internal sealed class ScrollToBottomInteraction : IInteractionStrategy
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
        }
        catch (Exception ex)
        {
            return await Task.FromResult(new Error($"Error occured: {ex.Message}"));
        }
        return await Task.FromResult(Result.Success());
    }

    private static bool IsEndOfPage(ref long initialHeight, ref long currentHeight) =>
        currentHeight == initialHeight;
}
