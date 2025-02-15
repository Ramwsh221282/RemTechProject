using System.Collections;
using RemTechCommon.Utils.Extensions;
using RemTechCommon.Utils.ResultPattern;

namespace WebDriver.Worker.Service.Contracts.BaseImplementations;

public sealed class WebElementPool : IReadOnlyCollection<WebElement>
{
    private readonly List<WebElement> _elements = [];
    public IReadOnlyCollection<WebElement> Elements => _elements;

    public void AddElement(WebElement element) => _elements.Add(element);

    public Result<WebElement> GetWebElement(Func<WebElement, bool> predicate)
    {
        var element = _elements.FirstOrDefault(predicate);
        return element == null ? new Error("No element with this predicate") : element;
    }

    public IEnumerable<WebElement> GetWebElements(Func<WebElement, bool> predicate) =>
        _elements.Where(predicate);

    public void Clear() => _elements.Clear();

    public IEnumerator<WebElement> GetEnumerator() => _elements.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => _elements.Count;

    public Result<WebElement> this[int index] => _elements.GetByIndex(index);
}
