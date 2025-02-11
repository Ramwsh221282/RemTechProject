using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using RemTechCommon.Utils.ResultPattern;

namespace WebDriver.Core.Models;

internal sealed class WebDriverFactory
{
    private readonly WebDriverOptionsFactory _optionsFactory;

    private readonly WebDriverExecutableManager _manager;

    private readonly string _chromeExecutablePath =
        WebDriverConstants.GetExpectedChromeDriverExecutablePath();

    public WebDriverFactory(
        WebDriverOptionsFactory optionsFactory,
        WebDriverExecutableManager manager
    )
    {
        _optionsFactory = optionsFactory;
        _manager = manager;
    }

    public Result<IWebDriver> Instantiate(out string profile)
    {
        profile = string.Empty;
        try
        {
            bool isExists = EnsureChromeDriverExists();
            if (!isExists)
            {
                Result<string> installation = InstallChromeDriver(isExists);
                if (installation.IsFailure)
                {
                    Error error = installation.Error;
                    return error;
                }
            }

            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;
            service.EnableVerboseLogging = false;
            service.EnableAppendLog = false;
            service.SuppressInitialDiagnosticInformation = true;
            service.DisableBuildCheck = true;
            IWebDriver instance = new ChromeDriver(
                service,
                _optionsFactory.Create(out string profilePath)
            );
            profile = profilePath;
            return Result<IWebDriver>.Success(instance);
        }
        catch (Exception ex)
        {
            return new Error($"Error occured during starting process. {ex.Message}");
        }
    }

    private bool EnsureChromeDriverExists()
    {
        try
        {
            return File.Exists(_chromeExecutablePath);
        }
        catch
        {
            return false;
        }
    }

    private Result<string> InstallChromeDriver(bool isExists)
    {
        if (isExists)
            return new Error("Conflict with installation. Driver already exists");

        if (Directory.Exists(WebDriverConstants.ChromeDriversCataloguePath))
            Directory.Delete(WebDriverConstants.ChromeDriversCataloguePath, true);

        Result<string> installation = _manager.Install();
        return installation;
    }
}
