using OpenQA.Selenium;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Core.Models.InteractionStrategies;
using WebDriver.Core.Models.InteractionStrategies.Implementations;
using WebDriver.Core.Models.SearchStrategies;
using WebDriver.Core.Models.WebDriverStates;
using WebDriver.Core.Models.WebDriverStates.States;

namespace WebDriver.Core.Models;

public sealed class WebDriverInstance
{
    private static readonly string script =
        @"                
                Object.defineProperty(navigator, 'webdriver', {
                    get: () => undefined,
                    configurable: true // Make it configurable so it can be redefined later if needed
                });

                Object.defineProperty(navigator, 'languages', { 
                     get: () => ['ru-RU', 'ru'] 
                });

                Object.defineProperty(navigator, 'vendor', { 
                     get: () => 'Google Inc.' 
                });

                Object.defineProperty(navigator, 'platform', { 
                    get: () => 'Win32' 
                });

                Object.defineProperty(navigator, 'mimeTypes', { 
                    get: () => [{ type: 'application/pdf' }, { type: 'application/x-google-chrome-pdf' }] 
                });

                const originalQuery = window.navigator.permissions.query;
                    window.navigator.permissions.__proto__.query = (parameters) => (
                        parameters.name === 'notifications' ?
                        Promise.resolve({ state: Notification.permission }) :
                        originalQuery(parameters)
                    );

                if (window.WebGLRenderingContext) {
                        const originalGetParameter = WebGLRenderingContext.prototype.getParameter;
                        WebGLRenderingContext.prototype.getParameter = function(parameter) {
                            // Constants for UNMASKED_VENDOR_WEBGL and UNMASKED_RENDERER_WEBGL
                            if (parameter === 37445) { 
                                return 'Google Inc.';
                            }
                            if (parameter === 37446) { 
                                return 'ANGLE (Intel(R) HD Graphics)';
                            }
                            return originalGetParameter.call(this, parameter);
                        };
                    }
                
                Object.defineProperty(navigator, 'plugins', {
                    get: () => ({
                        length: 0,
                        item: () => null,
                        namedItem: () => null
                    }),
                    configurable: true
                });               
                
                const originalUserAgent = navigator.userAgent;
                Object.defineProperty(navigator, 'userAgent', {
                    get: () => originalUserAgent.replace('HeadlessChrome', 'Chrome'),
                    configurable: true
                });
                
                delete window.__cdc_function_toString;                
                if (Element.prototype.hasOwnProperty('attachShadow')) {
                    delete Element.prototype.attachShadow;
                }
            ";

    private readonly WebElementObjectsPool _pool = new WebElementObjectsPool();
    private IWebDriverState _state = new NotInitializedState();
    private IWebDriver _instance = null!;
    public string ProfilePath { get; private set; } = string.Empty;

    internal void AddInPool(WebElementObject element) => _pool.RegisterObject(element);

    internal Result<WebElementObject> GetFromPool(Guid id) => _pool[id];

    public Result StopWebDriver()
    {
        StopDriverInteraction strategy = new StopDriverInteraction(_instance, _state);
        Result stopping = strategy.Perform(this);

        if (stopping.IsFailure)
            return stopping;

        _state = new NotInitializedState();
        return stopping;
    }

    public Result StartWebDriver(string loadStrategy)
    {
        StartDriverInteraction strategy = new StartDriverInteraction(loadStrategy);
        Result<(IWebDriver, string)> instantiation = strategy.Perform();

        if (instantiation.IsFailure)
            return instantiation.Error;

        _instance = instantiation.Value.Item1;
        ProfilePath = instantiation.Value.Item2;

        while (true)
        {
            try
            {
                ((IJavaScriptExecutor)_instance).ExecuteScript(script);
                break;
            }
            catch
            {
                // ignored
            }
        }

        _state = new SleepingState();
        return Result.Success();
    }

    internal Result<IWebDriver> GetRunningDriver()
    {
        if (!_state.CanExecuteAction)
            return WebDriverPluginErrors.StateError(_state);

        _state = new ProcessingState();
        return Result<IWebDriver>.Success(_instance);
    }

    public void RefreshPool() => _pool.Refresh();

    public async Task<Result<T>> PerformInteraction<T>(IInteractionStrategy<T> strategy)
    {
        Result<T> result = await strategy.Perform(this);

        if (_state is ProcessingState)
            _state = new SleepingState();

        return result;
    }

    public async Task<Result> PerformInteraction(IInteractionStrategy strategy)
    {
        Result result = await strategy.Perform(this);

        if (_state is ProcessingState)
            _state = new SleepingState();

        return result;
    }

    public async Task<Result<WebElementObject>> FindElement(ISingleElementSearchStrategy strategy)
    {
        Result<WebElementObject> result = await strategy.Search(this);

        if (_state is ProcessingState)
            _state = new SleepingState();

        return result;
    }

    public async Task<Result<WebElementObject[]>> FindElements(
        IMultipleElementSearchStrategy strategy
    )
    {
        Result<WebElementObject[]> result = await strategy.Search(this);

        if (_state is ProcessingState)
            _state = new SleepingState();

        return result;
    }
}
