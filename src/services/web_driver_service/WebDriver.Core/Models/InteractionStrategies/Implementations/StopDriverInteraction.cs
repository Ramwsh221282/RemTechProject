using System.Diagnostics;
using OpenQA.Selenium;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Core.Models.WebDriverStates;
using WebDriver.Core.Models.WebDriverStates.States;

namespace WebDriver.Core.Models.InteractionStrategies.Implementations;

internal sealed class StopDriverInteraction
{
    private readonly IWebDriver _instance;
    private readonly IWebDriverState _state;

    public StopDriverInteraction(IWebDriver driver, IWebDriverState state)
    {
        _instance = driver;
        _state = state;
    }

    public Result Perform(WebDriverInstance instance)
    {
        if (_state is not (SleepingState or ProcessingState))
            return WebDriverPluginErrors.StateError(_state);

        _instance.Close();
        _instance.Quit();
        _instance.Dispose();

        Process[] chromeDriverProcesses = Process.GetProcessesByName("chromedriver");
        foreach (Process process in chromeDriverProcesses)
            process.Kill(true);

        if (!string.IsNullOrWhiteSpace(instance.ProfilePath))
            Directory.Delete(instance.ProfilePath, true);

        return Result.Success();
    }
}
