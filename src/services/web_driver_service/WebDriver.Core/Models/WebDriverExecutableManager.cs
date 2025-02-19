using RemTechCommon.Utils.ResultPattern;
using WebDriverManager;

namespace WebDriver.Core.Models;

internal sealed class WebDriverExecutableManager
{
    private readonly DriverManager _manager = new DriverManager(
        downloadDirectory: WebDriverConstants.ChromeDriversCataloguePath
    );

    public Result<string> Install()
    {
        try
        {
            string created = _manager.SetUpDriver(
                WebDriverConstants.DriverConfig,
                WebDriverConstants.CompatibleVersion
            );

            if (!string.IsNullOrWhiteSpace(created))
                return created;
            Error error = new Error("Failed to install Web Driver");
            return error;
        }
        catch
        {
            Error error = new Error("Failed to install Web Driver");
            return error;
        }
    }

    public Result<string> Uninstall()
    {
        string path = WebDriverConstants.ExpectedChromeDriverPath;
        if (!Directory.Exists(path))
        {
            Error error = new Error($"Failed to uninstall Web Driver. No {path} Directory exists");
            return error;
        }

        try
        {
            Directory.Delete(path, true);
            return path;
        }
        catch (Exception ex)
        {
            return new Error($"Web Driver Uninstallation Failed With Exception. {ex.Message}");
        }
    }
}
