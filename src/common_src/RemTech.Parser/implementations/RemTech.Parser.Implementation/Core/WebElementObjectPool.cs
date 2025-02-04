using OpenQA.Selenium;
using RemTech.Parser.Contracts.Contracts;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTech.Parser.Implementation.Core;

public sealed class WebElementObjectPool
{
    private readonly List<WebElementObjectInternal> _elements = [];

    private readonly ILogger _logger;

    public WebElementObjectPool(ILogger logger) => _logger = logger;

    public int NextElementPosition { get; private set; }

    public void Refresh() => _elements.Clear();

    public void Add(WebElementObjectInternal element)
    {
        _elements.Add(element);
        NextElementPosition++;
        _logger.Warning(
            "Added element in Web Driver Elements Pool. Count: {Count}",
            NextElementPosition
        );
    }

    public Result<WebElementObjectInternal> this[int position]
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

    public WebElementObjectInternal(string path, string type, int position, IWebElement element)
        : base(path, type, position) => Element = element;

    public static bool operator ==(
        WebElementObjectInternal internalElement,
        WebElementObject externalElement
    ) =>
        internalElement.ElementPath == externalElement.ElementPath
        && internalElement.ElementPathType == externalElement.ElementPathType
        && internalElement.Position == externalElement.Position;

    public static bool operator !=(
        WebElementObjectInternal internalElement,
        WebElementObject externalElement
    ) => !(internalElement == externalElement);
}
