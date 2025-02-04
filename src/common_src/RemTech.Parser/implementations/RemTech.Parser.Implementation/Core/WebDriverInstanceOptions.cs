using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace RemTech.Parser.Implementation.Core;

public sealed class WebDriverInstanceOptions
{
    public ChromeOptions Options { get; }

    public WebDriverInstanceOptions()
    {
        Options = new ChromeOptions();
        Options.PageLoadStrategy = PageLoadStrategy.Eager;

        string profile = WebDriverPluginConstants.ProfilePath;
        if (Directory.Exists(profile))
        {
            StringBuilder profileOptionsBuilder = new();
            profileOptionsBuilder.Append("user-data-dir=");
            profileOptionsBuilder.Append(profile);
            Options.AddArgument(profileOptionsBuilder.ToString());
        }

        Options.AddArgument("--start-maximized");
        Options.AddArgument("--disable-blink-features=AutomationControlled");
        Options.AddExcludedArguments("excludeSwitches", "enable-automation");
        Options.AddArgument("--disable-web-security");
        Options.AddArgument("--ignore-certificate-errors");
        Options.AddArgument("--no-sandbox");
        Options.AddArgument("--disable-gpu");
        Options.AddArgument("--disable-dev-shm-usage");
        Options.AddArgument("--allow-running-insecure-content");
        Options.AddArgument("--disable-infobars");
        Options.AddArgument("--disable-extensions");
        Options.AddArgument("--disable-images");
        Options.AddArgument("--disable-popup-blocking");
        Options.AddArgument("--fast-enable");
        Options.AcceptInsecureCertificates = true;
        Options.AddAdditionalOption("useAutomationExtensions", false);
    }
}
