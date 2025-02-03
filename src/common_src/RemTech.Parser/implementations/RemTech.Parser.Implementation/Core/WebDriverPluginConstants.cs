using RemTechCommon.Constants;
using WebDriverManager.DriverConfigs.Impl;

namespace RemTech.Parser.Implementation.Core;

public static class WebDriverPluginConstants
{
    internal static ChromeConfig DriverConfig = new ChromeConfig();

    internal static readonly string CompatibleVersion = DriverConfig.GetLatestVersion();

    internal static readonly string PluginPath = Path.Combine(
        ApplicationPathConstants.PluginsDirectory,
        "WebDriverPlugin"
    );

    internal static readonly string ExpectedChromeDriverPath = Path.Combine(
        PluginPath,
        "Chrome",
        CompatibleVersion
    );

    internal static readonly string ProfilePath = Path.Combine(
        GetExpectedChromeDriverExecutableDirectory(),
        "Profile"
    );

    internal static string GetExpectedChromeDriverExecutableDirectory() =>
        Environment.Is64BitOperatingSystem
            ? Path.Combine(WebDriverPluginConstants.ExpectedChromeDriverPath, "X64")
            : Path.Combine(WebDriverPluginConstants.ExpectedChromeDriverPath, "X32");

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
