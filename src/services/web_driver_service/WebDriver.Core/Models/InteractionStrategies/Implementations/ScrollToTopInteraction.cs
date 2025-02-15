using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Core.Models.SearchStrategies;

namespace WebDriver.Core.Models.InteractionStrategies.Implementations;

internal sealed class ScrollToTopInteraction : BaseSearchElementStrategy, IInteractionStrategy
{
    private bool _isScrolled;

    private const string Script = "window.scrollTo(0, 0);";

    private const string GetScrollPositionScript =
        "return window.pageYOffset || document.documentElement.scrollTop || document.body.scrollTop || 0;";

    public async Task<Result> Perform(WebDriverInstance instance)
    {
        Result<IWebDriver> request = instance.GetRunningDriver();
        if (request.IsFailure)
            return await Task.FromResult(request);

        IWebDriver driver = request.Value;
        int attempts = 0;
        while (ReachedMaxAttempts(ref attempts))
        {
            try
            {
                while (!_isScrolled)
                {
                    long currentHeight = driver.ExecuteJavaScript<long>(GetScrollPositionScript);

                    if (currentHeight == 0)
                        _isScrolled = true;

                    driver.ExecuteJavaScript(Script);
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
            return await Task.FromResult(new Error("Cannot scroll to top."));

        return await Task.FromResult(Result.Success());
    }
}
