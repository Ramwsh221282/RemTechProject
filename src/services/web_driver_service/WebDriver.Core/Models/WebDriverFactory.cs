using OpenQA.Selenium.Chrome;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace WebDriver.Core.Models;

internal sealed class WebDriverFactory
{
    private readonly ILogger _logger;

    private readonly WebDriverOptionsFactory _optionsFactory;

    private readonly WebDriverExecutableManager _manager;

    private readonly WebDriverInstance _instance;

    private readonly string _chromeExecutablePath =
        WebDriverConstants.GetExpectedChromeDriverExecutablePath();

    public WebDriverFactory(
        ILogger logger,
        WebDriverOptionsFactory optionsFactory,
        WebDriverExecutableManager manager,
        WebDriverInstance instance
    )
    {
        _logger = logger;
        _optionsFactory = optionsFactory;
        _manager = manager;
        _instance = instance;
    }

    public Result Instantiate()
    {
        _logger.Information("Attempt to create web driver instance...");

        if (!_instance.IsDisposed)
        {
            Error error = new Error("Web driver instance is already running");
            _logger.Error("{Error}", error.Description);
            return error;
        }

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
        _instance.Instance = new ChromeDriver(
            service,
            _optionsFactory.Create(out string profilePath)
        );
        if (!string.IsNullOrWhiteSpace(profilePath))
            _instance.UniqueProfilePath = profilePath;
        _instance.IsDisposed = false;
        _instance.RefreshPool();
        return Result.Success();
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

        if (Directory.Exists(WebDriverConstants.ChromeDriversCataloguePath))
        {
            _logger.Information("Detected existing chrome drivers");
            Directory.Delete(WebDriverConstants.ChromeDriversCataloguePath, true);
            _logger.Information("Previous chrome drivers removed");
        }

        Result<string> installation = _manager.Install();
        return installation;
    }
}
