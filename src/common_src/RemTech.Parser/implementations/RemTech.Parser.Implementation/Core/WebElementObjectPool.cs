using OpenQA.Selenium;
using RemTech.Parser.Contracts.Contracts;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.Parser.Implementation.Core;

public sealed class WebElementObjectPool
{
    private readonly List<WebElementObjectInternal> _elements = [];
    public int LastElementIndex { get; private set; }

    public void Refresh() => _elements.Clear();

    public void Add(WebElementObjectInternal element)
    {
        _elements.Add(element);
        LastElementIndex++;
    }

    public Result<WebElementObject> this[int position]
    {
        get
        {
            if (position < 0 || position >= _elements.Count)
                return new Error("No such position in sequence");
            return _elements[position];
        }
    }
}

public sealed record WebElementObjectInternal : WebElementObject
{
    public IWebElement Element { get; }

    public WebElementObjectInternal(WebElementObject elementObject, IWebElement element)
        : base(elementObject.ElementPath, elementObject.ElementPathType, elementObject.Position) =>
        Element = element;
}
