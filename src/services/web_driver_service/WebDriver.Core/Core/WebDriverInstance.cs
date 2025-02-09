using OpenQA.Selenium;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Core.Queries.GetElement;

namespace WebDriver.Core.Core;

public sealed class WebDriverInstance
{
    private readonly ILogger _logger;

    private readonly WebDriverFactory _factory;

    private readonly WebElementObjectsPool _pool = new WebElementObjectsPool();

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
        _pool.Refresh();
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
        _pool.Refresh();
        return Result.Success();
    }

    public Result AddInPool(WebElementObject element) => _pool.RegisterObject(element);

    public Result<WebElementObject> GetFromPool(Guid id) => _pool[id];
}

internal static class WebDriverInstanceExtensions
{
    public static Result<WebElementObject[]> GetMultipleElements(
        this WebDriverInstance instance,
        Guid id,
        GetElementQuery query
    )
    {
        if (instance.IsDisposed || instance.Instance == null)
            return WebDriverPluginErrors.AlreadyDisposed;

        Result<WebElementObject> existing = instance.GetFromPool(id);
        if (existing.IsFailure)
            return existing.Error;

        Result<By> by = query.GetElementSearchType();
        if (by.IsFailure)
            return by.Error;

        try
        {
            IReadOnlyCollection<IWebElement> models = existing.Value.Model.FindElements(by.Value);
            if (models.Count == 0)
                return new Error(
                    $"No children elements with path: {query.Path} and {query.Type} found"
                );
            WebElementObject[] result = models
                .Select(m => new WebElementObject(query.Path, query.Type, m))
                .ToArray();

            for (int index = 0; index < result.Length; index++)
            {
                Result registration = instance.AddInPool(result[index]);
                if (registration.IsFailure)
                    return registration.Error;
            }

            return result;
        }
        catch
        {
            return new Error(
                $"No children elements with path: {query.Path} and {query.Type} found"
            );
        }
    }

    public static Result<WebElementObject> GetSingleElement(
        this WebDriverInstance driver,
        Guid id,
        GetElementQuery query
    )
    {
        if (driver.IsDisposed || driver.Instance == null)
            return WebDriverPluginErrors.AlreadyDisposed;

        Result<WebElementObject> existing = driver.GetFromPool(id);
        if (existing.IsFailure)
            return existing.Error;

        Result<By> by = query.GetElementSearchType();
        if (by.IsFailure)
            return by.Error;

        try
        {
            WebElementObject element = new WebElementObject(
                query.Path,
                query.Type,
                existing.Value.Model.FindElement(by)
            );
            Result registration = driver.AddInPool(element);
            return registration.IsSuccess ? element : registration.Error;
        }
        catch
        {
            return new Error(
                $"Cannot find child element with type: {query.Type} and path: {query.Path}"
            );
        }
    }

    public static Result<WebElementObject> GetSingleElement(
        this WebDriverInstance driver,
        GetElementQuery query
    )
    {
        if (driver.IsDisposed || driver.Instance == null)
            return WebDriverPluginErrors.AlreadyDisposed;

        Result<By> by = query.GetElementSearchType();
        if (by.IsFailure)
            return by.Error;

        try
        {
            WebElementObject result = new WebElementObject(
                query.Path,
                query.Type,
                driver.Instance.FindElement(by.Value)
            );
            Result registration = driver.AddInPool(result);
            return registration.IsSuccess ? result : registration.Error;
        }
        catch
        {
            return new Error($"Can't find element with path: {query.Path} and type: {query.Type}.");
        }
    }

    public static Result<By> GetElementSearchType(this GetElementQuery query)
    {
        Result<By> by = query switch
        {
            GetElementByClassQuery => By.ClassName(query.Path),
            GetElementByXPathQuery => By.XPath(query.Path),
            GetElementByTagQuery => By.TagName(query.Path),
            _ => new Error("Invalid query path"),
        };
        return by;
    }
}
