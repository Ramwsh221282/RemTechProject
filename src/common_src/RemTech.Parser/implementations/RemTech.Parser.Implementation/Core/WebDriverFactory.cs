using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTech.Parser.Implementation.Core;

public sealed class WebDriverFactory
{
    private readonly ILogger _logger;

    private readonly WebDriverInstanceOptions _options;

    private readonly WebDriverExecutableManager _manager;

    private readonly string _chromeExecutablePath =
        WebDriverPluginConstants.GetExpectedChromeDriverExecutablePath();

    public WebDriverFactory(
        ILogger logger,
        WebDriverInstanceOptions options,
        WebDriverExecutableManager manager
    )
    {
        _logger = logger;
        _options = options;
        _manager = manager;
        logger.Information("Web Driver Factory created");
    }

    public Result<IWebDriver> Create()
    {
        bool isExists = EnsureChromeDriverExists();
        if (!isExists)
        {
            Result<string> installation = InstallChromeDriver(isExists);
            if (installation.IsFailure)
            {
                Error error = installation.Error;
                _logger.Error("{Error}", error.Description);
                return error;
            }
            _logger.Information("Chrome driver installed. Path: {Path}", installation.Value);
        }

        ChromeDriverService service = ChromeDriverService.CreateDefaultService();
        service.HideCommandPromptWindow = true;
        service.EnableVerboseLogging = false;
        service.EnableAppendLog = false;

        IWebDriver driver = new ChromeDriver(service, _options.Options);
        return Result<IWebDriver>.Success(driver);
    }

    private bool EnsureChromeDriverExists()
    {
        try
        {
            if (File.Exists(_chromeExecutablePath))
                return true;
            _logger.Error("No web driver chrome directories");
            return false;
        }
        catch
        {
            _logger.Error("Web driver executable doesn't exist");
            return false;
        }
    }

    private Result<string> InstallChromeDriver(bool isExists)
    {
        if (isExists)
            return new Error("Conflict with installation. Driver already exists");

        if (Directory.Exists(WebDriverPluginConstants.ChromeDriversCataloguePath))
        {
            _logger.Information("Detected existing chrome drivers");
            Directory.Delete(WebDriverPluginConstants.ChromeDriversCataloguePath, true);
            _logger.Information("Previous chrome drivers removed");
        }

        Result<string> installation = _manager.Install();
        return installation;
    }
}
