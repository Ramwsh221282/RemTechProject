using Serilog;

namespace WebDriver.Core.Core;

public abstract class BaseWebDriverHandler
{
    protected readonly WebDriverInstance _instance;
    protected readonly ILogger _logger;

    public BaseWebDriverHandler(WebDriverInstance instance, ILogger logger)
    {
        _instance = instance;
        _logger = logger;
    }
}
