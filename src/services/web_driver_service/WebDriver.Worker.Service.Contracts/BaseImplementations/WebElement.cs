using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours;
using WebDriver.Worker.Service.Contracts.Responses;

namespace WebDriver.Worker.Service.Contracts.BaseImplementations;

public sealed record WebElement(WebElementResponse Model, string Name)
{
    private readonly Dictionary<string, string> _attributes = [];
    private readonly List<WebElement> _childs = [];
    public WebElement? Parent { get; private set; }
    public IReadOnlyDictionary<string, string> Attributes => _attributes;
    public IReadOnlyCollection<WebElement> Childs => _childs;
    public string Text { get; private set; } = String.Empty;

    public Result SetAttribute(string attributeName, string attributeValue)
    {
        bool containsKey = _attributes.ContainsKey(attributeName);
        if (containsKey)
            return new Error("Element contains this key");

        _attributes.Add(attributeName, attributeValue);
        return Result.Success();
    }

    public void SetText(string text) => Text = text;

    public WebElementPool AsPool(Func<WebElement, bool> predicate)
    {
        IEnumerable<WebElement> filtered = _childs.Where(predicate);
        WebElementPool pool = new();
        foreach (WebElement element in filtered)
            pool.AddElement(element);

        return pool;
    }

    public WebElementPool AsPool()
    {
        WebElementPool pool = new();
        _childs.ForEach(c => pool.AddElement(c));

        return pool;
    }

    public void ExcludeChilds(Predicate<WebElement> predicate) => _childs.RemoveAll(predicate);

    public async Task<Result> ExecuteForChilds(
        IMessagePublisher publisher,
        Func<WebElement, IWebDriverBehavior> factory,
        CancellationToken ct = default
    )
    {
        Stack<WebElement> stack = new();
        foreach (WebElement child in this.Childs.Reverse())
            stack.Push(child);

        while (stack.Count > 0)
        {
            WebElement current = stack.Pop();
            IWebDriverBehavior behavior = factory(current);
            Result execution = await behavior.Execute(publisher, ct);

            if (execution.IsFailure)
                return execution;

            foreach (WebElement child in current.Childs.Reverse())
                stack.Push(child);
        }

        return Result.Success();
    }

    public async Task<Result> ExecuteForChilds(
        IMessagePublisher publisher,
        Func<WebElement, IWebDriverBehavior> factory,
        Func<WebElement, bool> predicate,
        CancellationToken ct = default
    )
    {
        Stack<WebElement> stack = new();
        foreach (WebElement child in this.Childs.Where(predicate).Reverse())
            stack.Push(child);

        while (stack.Count > 0)
        {
            WebElement current = stack.Pop();
            IWebDriverBehavior behavior = factory(current);
            Result execution = await behavior.Execute(publisher, ct);

            if (execution.IsFailure)
                return execution;

            foreach (WebElement child in current.Childs.Where(predicate).Reverse())
                stack.Push(child);
        }

        return Result.Success();
    }

    public static WebElement FromParent(WebElement parent, WebElementResponse child, string name)
    {
        WebElement element = new WebElement(child, name) { Parent = parent };
        parent._childs.Add(element);

        return element;
    }

    public static IEnumerable<WebElement> FromParent(
        WebElement parent,
        IEnumerable<WebElementResponse> childs,
        string name
    )
    {
        List<WebElement> elements = [];
        elements.AddRange(childs.Select(child => parent.FromParent(child, name)));

        return elements;
    }
}

public static class AvitoWebElementExtensions
{
    public static WebElement FromParent(
        this WebElement parent,
        WebElementResponse child,
        string name
    ) => WebElement.FromParent(parent, child, name);

    public static IEnumerable<WebElement> FromParent(
        this WebElement parent,
        IEnumerable<WebElementResponse> childs,
        string name
    ) => WebElement.FromParent(parent, childs, name);
}
