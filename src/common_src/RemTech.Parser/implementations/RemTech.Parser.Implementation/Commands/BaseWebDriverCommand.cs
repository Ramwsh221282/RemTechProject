using RemTech.Parser.Implementation.Core;
using Serilog;

namespace RemTech.Parser.Implementation.Commands;

public abstract class BaseWebDriverCommand
{
    protected readonly WebDriverInstance _instance;
    protected readonly ILogger _logger;

    public BaseWebDriverCommand(WebDriverInstance instance, ILogger logger)
    {
        _instance = instance;
        _logger = logger;
    }
}
