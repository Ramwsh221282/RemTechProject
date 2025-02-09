using WebDriverManager.DriverConfigs.Impl;

namespace WebDriver.Core.Core;

internal static class WebDriverPluginConstants
{
    internal static ChromeConfig DriverConfig = new ChromeConfig();

    internal static readonly string CompatibleVersion = DriverConfig.GetMatchingBrowserVersion();

    internal static readonly string ChromeDriversCataloguePath = Path.Combine(
        Environment.CurrentDirectory,
        "WebDrivers"
    );

    internal static readonly string ExpectedChromeDriverPath = Path.Combine(
        ChromeDriversCataloguePath,
        "Chrome",
        CompatibleVersion
    );

    internal static readonly string ProfilePath = Path.Combine(
        ChromeDriversCataloguePath,
        "Profile"
    );

    internal static string GetExpectedChromeDriverExecutablePath() =>
        Environment.Is64BitOperatingSystem
            ? Path.Combine(
                WebDriverPluginConstants.ExpectedChromeDriverPath,
                "X64",
                "chromedriver.exe"
            )
            : Path.Combine(
                WebDriverPluginConstants.ExpectedChromeDriverPath,
                "X32",
                "chromedriver.exe"
            );
}
