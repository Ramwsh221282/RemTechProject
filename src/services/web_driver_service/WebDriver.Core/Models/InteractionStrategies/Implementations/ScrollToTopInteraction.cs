using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using RemTechCommon.Utils.ResultPattern;

namespace WebDriver.Core.Models.InteractionStrategies.Implementations;

internal sealed class ScrollToTopInteraction : IInteractionStrategy
{
    private const string Script = "window.scrollTo(0, 0);";

    private const string GetScrollPositionScript =
        "return window.pageYOffset || document.documentElement.scrollTop || document.body.scrollTop || 0;";

    public async Task<Result> Perform(WebDriverInstance instance)
    {
        Result<IWebDriver> request = instance.GetRunningDriver();
        if (request.IsFailure)
            return request;

        IWebDriver driver = request.Value;
        bool isScrolled = false;
        while (!isScrolled)
        {
            long currentHeight = driver.ExecuteJavaScript<long>(GetScrollPositionScript);

            if (currentHeight == 0)
                isScrolled = true;

            driver.ExecuteJavaScript(Script);
        }

        return await Task.FromResult(Result.Success());
    }
}
