using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriverManager;

namespace WebDriver.Core.Models;

internal sealed class WebDriverExecutableManager
{
    private readonly ILogger _logger;

    public WebDriverExecutableManager(ILogger logger)
    {
        _logger = logger;
        _logger.Information("Web Driver Executable Manager Created");
    }

    private readonly DriverManager _manager = new DriverManager(
        downloadDirectory: WebDriverConstants.ChromeDriversCataloguePath
    );

    public Result<string> Install()
    {
        _logger.Information("Starting Web Driver Installation Process");
        try
        {
            string created = _manager.SetUpDriver(
                WebDriverConstants.DriverConfig,
                WebDriverConstants.CompatibleVersion
            );

            if (string.IsNullOrWhiteSpace(created))
            {
                Error error = new Error("Failed to install Web Driver");
                _logger.Error("{Message}", error.Description);
                return error;
            }

            _logger.Information("Web Driver Installation Completed. Path: {Path}", created);
            return created;
        }
        catch
        {
            Error error = new Error("Failed to install Web Driver");
            _logger.Error("{Message}", error.Description);
            return error;
        }
    }

    public Result<string> Uninstall()
    {
        string path = WebDriverConstants.ExpectedChromeDriverPath;
        if (!Directory.Exists(path))
        {
            Error error = new Error($"Failed to uninstall Web Driver. No {path} Directory exists");
            _logger.Error("{Message}", error.Description);
            return error;
        }

        try
        {
            Directory.Delete(path, true);
            _logger.Information("Web Driver Uninstallation Completed");
            return path;
        }
        catch (Exception ex)
        {
            _logger.Information(
                "Web Driver Uninstallation Failed. Exception: {Exception}",
                ex.Message
            );
            return new Error("Web Driver Uninstallation Failed With Exception.");
        }
    }
}
