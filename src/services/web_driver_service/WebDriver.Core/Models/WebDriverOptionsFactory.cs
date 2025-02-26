using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using RemTechCommon.Utils.ResultPattern;

namespace WebDriver.Core.Models;

internal sealed class WebDriverOptionsFactory
{
    private readonly string _loadStrategy;
    private static readonly Lock _lock = new();

    public WebDriverOptionsFactory(string loadStrategy)
    {
        _loadStrategy = loadStrategy;
    }

    public Result<ChromeOptions> Create(out string uniqueProfilePath)
    {
        uniqueProfilePath = string.Empty;
        var strategy = ParseStrategy(_loadStrategy);
        if (!Enum.IsDefined(typeof(PageLoadStrategy), strategy))
        {
            var error = new Error("Page load strategy is not supported");
            return error;
        }

        var resolved = (PageLoadStrategy)strategy;
        var options = new ChromeOptions { PageLoadStrategy = resolved };
        options.AddExcludedArgument("enable-automation");
        options.AddArgument("--start-maximized");
        options.AddArgument("--disable-blink-features=AutomationControlled");
        options.AddExcludedArguments("excludeSwitches", "enable-automation");
        options.AddArgument("--disable-web-security");
        options.AddArgument("--ignore-certificate-errors");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-gpu");
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--allow-running-insecure-content");
        options.AddArgument("--disable-infobars");
        options.AddArgument("--disable-extensions");
        options.AddArgument("--disable-images");
        options.AddArgument("--disable-popup-blocking");
        options.AddArgument("--fast-enable");
        options.AddArgument("--disable-logging");
        options.AddArgument("--log-level=3");
        options.AddArgument("--headless=new");
        options.AddArgument("--disable-background-networking");
        options.AddArgument("disable-selenium-metadata");
        options.AddArgument("--disable-sync");
        options.AddArgument("--disable-translate");
        options.AddArgument("--disable-default-apps");
        options.AddArgument("--disable-hang-monitor");
        options.AddArgument("--disable-prompt-on-repost");
        options.AcceptInsecureCertificates = true;
        options.AddAdditionalOption("useAutomationExtensions", false);
        if (!Directory.Exists(WebDriverConstants.ProfilePath))
            return options;
        CreateUniqueProfileFromExisting(out var sessionProfilePath);
        if (string.IsNullOrWhiteSpace(sessionProfilePath))
            return options;
        Console.WriteLine(sessionProfilePath);
        options.AddArgument($"--user-data-dir={sessionProfilePath}");
        uniqueProfilePath = sessionProfilePath;

        return options;
    }

    private static int ParseStrategy(string strategyType)
    {
        var strategy = strategyType switch
        {
            "none" => 3,
            "eager" => 2,
            "normal" => 1,
            "default" => 0,
            _ => -1,
        };
        return strategy;
    }

    private static void CreateUniqueProfileFromExisting(out string profilePath)
    {
        var id = Guid.NewGuid();

        var parentPath = Path.GetDirectoryName(WebDriverConstants.ProfilePath);
        if (string.IsNullOrEmpty(parentPath))
        {
            profilePath = "";
            return;
        }

        var uniquePath = Path.Combine(parentPath, id.ToString());
        CopyDirectory(WebDriverConstants.ProfilePath, uniquePath);
        profilePath = uniquePath;
    }

    private static void CopyDirectory(string sourceDir, string destDir)
    {
        Directory.CreateDirectory(destDir);

        var files = Directory.GetFiles(sourceDir);
        Parallel.ForEach(
            files,
            file =>
            {
                var destFile = Path.Combine(destDir, Path.GetFileName(file));
                lock (_lock)
                {
                    File.Copy(file, destFile, true);
                }
            }
        );

        var dirs = Directory.GetDirectories(sourceDir);
        Parallel.ForEach(
            dirs,
            dir =>
            {
                var destSubDir = Path.Combine(destDir, Path.GetFileName(dir));
                CopyDirectory(dir, destSubDir);
            }
        );
    }
}
