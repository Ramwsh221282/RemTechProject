using Serilog;
using WebDriver.Core.Models;

namespace WebDriver.Application.Handlers;

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
