using OpenQA.Selenium;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTech.Parser.Implementation.Core;

public sealed class WebDriverInstance
{
    private readonly ILogger _logger;

    private readonly WebDriverFactory _factory;
    public bool IsDisposed { get; private set; }
    public IWebDriver? Instance { get; private set; }

    public WebDriverInstance(ILogger logger, WebDriverFactory factory)
    {
        _logger = logger;
        _factory = factory;
    }

    public Result Instantiate()
    {
        IsDisposed = false;
        Result<IWebDriver> instance = _factory.Create();

        if (instance.IsFailure)
        {
            Error error = instance.Error;
            _logger.Error("{Error}", error.Description);
            return instance.Error;
        }
        Instance = instance.Value;
        _logger.Information("Instantiated new web driver instance");
        return Result.Success();
    }

    public Result Dispose()
    {
        if (IsDisposed || Instance == null)
        {
            Error error = WebDriverPluginErrors.AlreadyDisposed;
            _logger.Error("{Error}", error.Description);
            return error;
        }

        Instance.Dispose();
        Instance = null;
        IsDisposed = true;
        _logger.Information("Disposed web driver instance");
        return Result.Success();
    }
}
