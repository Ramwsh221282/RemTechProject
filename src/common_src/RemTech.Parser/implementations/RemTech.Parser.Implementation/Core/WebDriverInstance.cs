using System.Text;
using OpenQA.Selenium;
using RemTech.Parser.Contracts.Contracts;
using RemTech.Parser.Contracts.Contracts.Queries;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTech.Parser.Implementation.Core;

public sealed class WebDriverInstance
{
    private readonly ILogger _logger;

    private readonly WebDriverFactory _factory;

    private readonly WebElementObjectPool _elements = new WebElementObjectPool();
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

    public void AddElement(IWebElement element, WebElementObject founded)
    {
        WebElementObjectInternal internalElement = new WebElementObjectInternal(founded, element);
        _elements.Add(internalElement);
    }

    public Result<WebElementObject> GetElement(int position) => _elements[position];

    public int GetLastPosition() => _elements.LastElementIndex;
}

public static class WebDriverInstanceExtensions
{
    public static Result<WebElementObject> GetElement(
        this WebDriverInstance driver,
        GetElementQuery query
    )
    {
        if (driver.IsDisposed || driver.Instance == null)
            return WebDriverPluginErrors.AlreadyDisposed;

        Result<By> by = query switch
        {
            GetElementByClassQuery => By.ClassName(query.Path),
            GetElementByXPathQuery => By.XPath(query.Path),
            _ => new Error("Invalid query path"),
        };

        if (by.IsFailure)
            return by.Error;

        try
        {
            IWebElement element = driver.Instance.FindElement(by.Value);
            WebElementObject foundedObject = new WebElementObject(
                query.Path,
                query.Type,
                driver.GetLastPosition()
            );
            driver.AddElement(element, foundedObject);
            return foundedObject;
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
}
