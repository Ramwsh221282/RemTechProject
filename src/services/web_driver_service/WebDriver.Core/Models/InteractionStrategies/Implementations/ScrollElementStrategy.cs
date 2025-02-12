using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using RemTechCommon.Utils.ResultPattern;

namespace WebDriver.Core.Models.InteractionStrategies.Implementations;

internal sealed class ScrollElementStrategy : IInteractionStrategy
{
    private readonly Guid _elementId;

    public ScrollElementStrategy(Guid elementId) => _elementId = elementId;

    public async Task<Result> Perform(WebDriverInstance instance)
    {
        Result<IWebDriver> request = instance.GetRunningDriver();
        if (request.IsFailure)
            return request.Error;

        Result<WebElementObject> element = instance.GetFromPool(_elementId);
        if (element.IsFailure)
            return element.Error;

        IWebDriver driver = request.Value;
        IWebElement model = element.Value.Model;
        long currentHeight = driver.ExecuteJavaScript<long>(
            "return arguments[0].scrollHeight",
            model
        );
        while (true)
        {
            driver.ExecuteJavaScript("arguments[0].scrollTop = arguments[0].scrollHeight", model);
            await Task.Delay(TimeSpan.FromSeconds(5));

            driver.ExecuteJavaScript("arguments[0].scrollTop = 0", model);
            await Task.Delay(TimeSpan.FromSeconds(5));

            long newHeight = driver.ExecuteJavaScript<long>(
                "return arguments[0].scrollHeight",
                model
            );

            if (newHeight == currentHeight)
                break;

            currentHeight = newHeight;
        }

        return Result.Success();
    }
}
