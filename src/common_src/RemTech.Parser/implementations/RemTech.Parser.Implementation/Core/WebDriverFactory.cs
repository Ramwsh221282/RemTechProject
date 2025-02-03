using RemTech.Parser.Contracts.Contracts;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTech.Parser.Implementation.Core;

public sealed class WebDriverFactory : IWebDriverFactory
{
    private readonly ILogger _logger;

    private readonly string _chromeExecutablePath =
        WebDriverPluginConstants.GetExpectedChromeDriverExecutablePath();

    public WebDriverFactory(ILogger logger)
    {
        _logger = logger;
        logger.Information("Web Driver Factory created");
    }

    public Result<IWebDriverInstance> Create()
    {
        bool isExists = EnsureChromeDriverExists();
        if (!isExists)
            return new Error(
                "Cannot instantiate web driver instance. No web driver executable exists"
            );

        IWebDriverInstance instance = new WebDriverInstance(_logger);
        instance.Instantiate();
        _logger.Information("New Web driver instance instantiated");
        return Result<IWebDriverInstance>.Success(instance);
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
}
