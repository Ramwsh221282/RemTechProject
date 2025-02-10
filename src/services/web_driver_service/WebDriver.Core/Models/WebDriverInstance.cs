using OpenQA.Selenium;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Core.Models.InteractionStrategies;
using WebDriver.Core.Models.SearchStrategies;

namespace WebDriver.Core.Models;

public sealed class WebDriverInstance
{
    private readonly WebElementObjectsPool _pool = new WebElementObjectsPool();
    internal bool IsDisposed { get; set; } = true;
    internal IWebDriver? Instance { get; set; }

    internal string UniqueProfilePath { get; set; } = string.Empty;

    internal Result AddInPool(WebElementObject element) => _pool.RegisterObject(element);

    internal Result<WebElementObject> GetFromPool(Guid id) => _pool[id];

    internal Result<IWebDriver> GetRunningDriver()
    {
        if (Instance == null || IsDisposed)
            return WebDriverPluginErrors.Disposed;
        return Result<IWebDriver>.Success(Instance);
    }

    public void RefreshPool() => _pool.Refresh();

    public async Task<Result<T>> PerformInteraction<T>(IInteractionStrategy<T> strategy) =>
        await strategy.Perform(this);

    public async Task<Result> PerformInteraction(IInteractionStrategy strategy) =>
        await strategy.Perform(this);

    public async Task<Result<WebElementObject>> FindElement(
        ISingleElementSearchStrategy strategy
    ) => await strategy.Search(this);

    public async Task<Result<WebElementObject[]>> FindElements(
        IMultipleElementSearchStrategy strategy
    ) => await strategy.Search(this);
}
