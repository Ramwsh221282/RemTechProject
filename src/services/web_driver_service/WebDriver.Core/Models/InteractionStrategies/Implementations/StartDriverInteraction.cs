using OpenQA.Selenium;
using RemTechCommon.Utils.ResultPattern;

namespace WebDriver.Core.Models.InteractionStrategies.Implementations;

internal sealed class StartDriverInteraction
{
    private readonly WebDriverOptionsFactory _optionsFactory;
    private readonly WebDriverExecutableManager _manager;

    public StartDriverInteraction(string loadStrategy)
    {
        _manager = new WebDriverExecutableManager();
        _optionsFactory = new(loadStrategy);
    }

    public Result<(IWebDriver, string)> Perform()
    {
        WebDriverFactory factory = new WebDriverFactory(_optionsFactory, _manager);
        Result<IWebDriver> driver = factory.Instantiate(out string profile);
        if (driver.IsFailure)
            return driver.Error;
        return (driver.Value, profile);
    }
}
