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
                // 1. Overwrite the webdriver property
                Object.defineProperty(navigator, 'webdriver', {
                    get: () => false,
                    configurable: true // Make it configurable so it can be redefined later if needed
                });

                // 2. Redefine plugins and mimeTypes to return empty arrays
                Object.defineProperty(navigator, 'plugins', {
                    get: () => ({
                        length: 0,
                        item: () => null,
                        namedItem: () => null
                    }),
                    configurable: true
                });

                Object.defineProperty(navigator, 'mimeTypes', {
                    get: () => ({
                        length: 0,
                        item: () => null,
                        namedItem: () => null
                    }),
                    configurable: true
                });

                // 3. Modify userAgent, making it configurable is important if other scripts might interact with it
                const originalUserAgent = navigator.userAgent;
                Object.defineProperty(navigator, 'userAgent', {
                    get: () => originalUserAgent.replace('HeadlessChrome', 'Chrome'),
                    configurable: true
                });

                // 4. Delete potential Headless Chrome indicators
                delete window.__cdc_function_toString;
                // Be cautious with deleting browser built-ins like attachShadow. It might break functionality.
                // If you must, check if it exists first.
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
                continue;
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
