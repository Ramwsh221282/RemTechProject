using System.Security.Principal;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace RemTech.Parser.Implementation.Core;

public sealed class WebDriverInstanceOptions
{
    public ChromeOptions Options { get; }

    public WebDriverInstanceOptions()
    {
        string profile = WebDriverPluginConstants.ProfilePath;
        StringBuilder profileOptionsBuilder = new();
        profileOptionsBuilder.Append("user-data-dir=");
        profileOptionsBuilder.Append(profile);
        Options = new ChromeOptions();
        Options.PageLoadStrategy = PageLoadStrategy.Eager;
        Options.AddArgument(profileOptionsBuilder.ToString());
        Options.AddArgument("--start-maximized");
        Options.AddArgument("--disable-blink-features=AutomationControlled");
        Options.AddExcludedArguments("excludeSwitches", "enable-automation");
        Options.AddArgument("--disable-web-security");
        Options.AddArgument("--ignore-certificate-errors");
        Options.AddArgument("--no-sandbox");
        Options.AddArgument("--disable-gpu");
        Options.AddArgument("--disable-dev-shm-usage");
        Options.AddArgument("--allow-running-insecure-content");
        Options.AddArgument("--disable-infobars"); // Отключение всплывающих окон информации о браузере
        Options.AddArgument("--disable-extensions"); // Отключение всех расширений
        Options.AddArgument("--disable-images"); // Отключение загрузки изображений (если не требуются)
        Options.AddArgument("--disable-popup-blocking"); // Отключение блокировки всплывающих окон
        Options.AddArgument("--fast-enable"); // Включение ускоренной работы JavaScript (если доступно)
        Options.AcceptInsecureCertificates = true;
        Options.AddAdditionalOption("useAutomationExtensions", false);

        // Options = new ChromeOptions();
        // Options.PageLoadStrategy = PageLoadStrategy.None;
        // Options.AddArgument(WebDriverPluginConstants.ChromeUserDataDirectory);
        // Options.AddArgument("--start-maximized");
        // Options.AddArgument("--disable-blink-features=AutomationControlled");
        // Options.AddArgument("--disable-web-securtity");
        // Options.AddArgument("--ignore-certificate-errors");
        // Options.AddArgument("--no-sandbox");
        // Options.AddArgument("--disable-gpu");
        // Options.AddArgument("-disable-dev-shm-usage");
        // Options.AddArgument("--allow-running-insecure-content");
        // Options.AddArgument("--disable-extensions");
        // Options.AcceptInsecureCertificates = true;
        // Options.AddAdditionalOption("useAutomationExtensions", false);
    }

    private string GetChromeUserAppDataPath()
    {
        WindowsIdentity? identity = WindowsIdentity.GetCurrent();
        if (identity == null)
            throw new InvalidOperationException("No active windows identity");
        string lastUserName = identity.Name.Split('\\').Last();
        return "user-data-dir=C:\\Users\\"
            + lastUserName
            + "\\AppData\\Local\\Google\\Chrome\\User Data";
    }
}
