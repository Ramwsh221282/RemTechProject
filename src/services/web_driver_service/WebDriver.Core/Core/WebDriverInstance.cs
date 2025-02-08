using System.Text;
using OpenQA.Selenium;
using RemTech.WebDriver.Plugin.Queries.GetElement;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTech.WebDriver.Plugin.Core;

internal sealed class WebDriverInstance
{
    private readonly ILogger _logger;

    private readonly WebDriverFactory _factory;

    private readonly WebElementObjectPool _elements;
    public bool IsDisposed { get; private set; }
    public IWebDriver? Instance { get; private set; }

    public WebDriverInstance(ILogger logger, WebDriverFactory factory)
    {
        _logger = logger;
        _factory = factory;
        _elements = new WebElementObjectPool(_logger);
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

        _elements.Refresh();
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
        _elements.Refresh();
        return Result.Success();
    }

    public WebElementObject AddElement(IWebElement element, GetElementQuery query)
    {
        WebElementObjectInternal internalElement = new WebElementObjectInternal(
            query.Path,
            query.Type,
            _elements.NextElementPosition,
            element
        );
        _elements.Add(internalElement);
        return internalElement;
    }

    public Result<WebElementObjectInternal> GetExistingElement(int position) => _elements[position];

    public Result<WebElementObjectInternal> GetExistingElement(WebElementObject element) =>
        _elements[element.Position];
}

internal static class WebDriverInstanceExtensions
{
    public static Result<WebElementObject[]> GetElements(
        this WebDriverInstance instance,
        WebElementObjectInternal internalElement,
        GetElementQuery query
    )
    {
        if (instance.IsDisposed || instance.Instance == null)
            return WebDriverPluginErrors.AlreadyDisposed;

        Result<By> by = query.GetElementSearchType();
        if (by.IsFailure)
            return by.Error;

        try
        {
            IWebElement element = internalElement.Element;
            IReadOnlyCollection<IWebElement> elements = element.FindElements(by.Value);
            WebElementObject[] array = elements
                .Select(el => instance.AddElement(el, query))
                .ToArray();
            return array;
        }
        catch
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Failed to get elements with path: ");
            stringBuilder.Append(query.Path);
            stringBuilder.Append(" and type");
            stringBuilder.Append(query.Type);
            stringBuilder.Append(" do not exist.");
            return new Error(stringBuilder.ToString());
        }
    }

    public static Result<WebElementObject> GetElement(
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
            IWebElement element = driver.Instance.FindElement(by.Value);
            return driver.AddElement(element, query);
        }
        catch
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("The element with path: ");
            stringBuilder.Append(query.Path);
            stringBuilder.Append(" and type");
            stringBuilder.Append(query.Type);
            stringBuilder.Append(" does not exist.");
            return new Error(stringBuilder.ToString());
        }
    }

    public static Result<WebElementObject> GetElement(
        this WebDriverInstance driver,
        WebElementObjectInternal internalElement,
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
            IWebElement element = internalElement.Element.FindElement(by.Value);
            return driver.AddElement(element, query);
        }
        catch
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("The element with path: ");
            stringBuilder.Append(query.Path);
            stringBuilder.Append(" and type");
            stringBuilder.Append(query.Type);
            stringBuilder.Append(" does not exist.");
            return new Error(stringBuilder.ToString());
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
