using RemTechCommon.Utils.ResultPattern;
using WebDriver.Core.Models.WebDriverStates;

namespace WebDriver.Core.Models;

internal static class WebDriverPluginErrors
{
    public static Error StateError(IWebDriverState state) =>
        new Error($"Cannot execute action. Web Driver State is: {state.StateName}");
}
