using System.Security.Principal;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace RemTech.Parser.Implementation.Core;

public sealed class WebDriverInstanceOptions
{
    private readonly ChromeOptions _options;

    public WebDriverInstanceOptions()
    {
        _options = new ChromeOptions();
        _options.PageLoadStrategy = PageLoadStrategy.None;
        _options.AddArgument(WebDriverPluginConstants.ChromeUserDataDirectory);
        _options.AddArgument("--start-maximized");
        _options.AddArgument("--disable-blink-features=AutomationControlled");
        _options.AddArgument("--disable-web-securtity");
        _options.AddArgument("--ignore-certificate-errors");
        _options.AddArgument("--no-sandbox");
        _options.AddArgument("--disable-gpu");
        _options.AddArgument("-disable-dev-shm-usage");
        _options.AddArgument("--allow-running-insecure-content");
        _options.AddArgument("--disable-extensions");
        _options.AcceptInsecureCertificates = true;
        _options.AddAdditionalOption("useAutomationExtensions", false);
    }

    public IWebDriver InstantiateDriverWithOptions() => new ChromeDriver(_options);
}
