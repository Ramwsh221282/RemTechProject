using OpenQA.Selenium;
using RemTech.Parser.Contracts.Contracts;
using Serilog;

namespace RemTech.Parser.Implementation.Core;

public sealed class WebDriverInstance : IWebDriverInstance
{
    private readonly WebDriverInstanceOptions _options = new WebDriverInstanceOptions();
    private readonly ILogger _logger;
    private IWebDriver? _instance;
    public bool IsDisposed { get; private set; }

    public WebDriverInstance(ILogger logger) => _logger = logger;

    public void Instantiate()
    {
        IsDisposed = false;
        _instance = _options.InstantiateDriverWithOptions();
        _logger.Information("Instantiated new web driver instance");
    }

    public void Dispose()
    {
        if (IsDisposed)
        {
            _logger.Error("The web driver instance has already been instantiated");
            return;
        }

        if (_instance == null)
        {
            _logger.Error("The web driver instance has already been instantiated");
            return;
        }

        _instance.Dispose();
        _instance = null;
        IsDisposed = true;
        _logger.Information("Disposed web driver instance");
    }
}
