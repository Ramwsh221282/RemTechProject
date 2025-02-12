using RemTechCommon.Utils.ResultPattern;

namespace WebDriver.Worker.Service.Contracts.BaseImplementations;

public sealed class WebElementPool
{
    private readonly List<WebElement> _elements = [];
    public IReadOnlyCollection<WebElement> Elements => _elements;

    public void AddElement(WebElement element) => _elements.Add(element);

    public Result<WebElement> GetWebElement(Func<WebElement, bool> predicate)
    {
        var element = _elements.FirstOrDefault(predicate);
        return element == null ? new Error("No element with this predicate") : element;
    }
}
