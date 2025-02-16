using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours;
using WebDriver.Worker.Service.Contracts.Responses;

namespace WebDriver.Worker.Service.Contracts.BaseImplementations;

public sealed record WebElement
{
    private List<WebElement> _childs = [];
    public WebElement? Parent { get; private set; }
    public IReadOnlyCollection<WebElement> Childs => _childs;
    public string Name { get; }
    public Guid Id { get; }
    public string InnerText { get; }
    public string OuterHTML { get; }

    public WebElement(WebElementResponse model, string name)
    {
        Id = model.ElementId;
        InnerText = model.InnerText;
        OuterHTML = model.OuterHTML;
        Name = name;
    }

    public void ExcludeChilds(Predicate<WebElement> predicate)
    {
        List<WebElement> newCollection = [];
        for (int index = 0; index < _childs.Count; index++)
        {
            if (predicate(_childs[index]))
                newCollection.Add(_childs[index]);
        }

        _childs = newCollection;
    }

    public async Task<Result> ExecuteForChilds(
        IMessagePublisher publisher,
        Func<WebElement, IWebDriverBehavior> factory,
        CancellationToken ct = default
    )
    {
        Stack<WebElement> stack = new();

        for (int index = _childs.Count - 1; index >= 0; index--)
            stack.Push(_childs[index]);

        while (stack.Count > 0)
        {
            WebElement current = stack.Pop();
            IWebDriverBehavior behavior = factory(current);
            Result execution = await behavior.Execute(publisher, ct);

            if (execution.IsFailure)
                return execution;

            for (int index = current._childs.Count - 1; index >= 0; index--)
                stack.Push(current._childs[index]);
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

        for (int index = _childs.Count - 1; index >= 0; index--)
        {
            if (predicate(_childs[index]))
                stack.Push(_childs[index]);
        }

        while (stack.Count > 0)
        {
            WebElement current = stack.Pop();
            IWebDriverBehavior behavior = factory(current);
            Result execution = await behavior.Execute(publisher, ct);

            if (execution.IsFailure)
                return execution;

            for (int index = current._childs.Count - 1; index >= 0; index--)
            {
                if (predicate(current._childs[index]))
                    stack.Push(current._childs[index]);
            }
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

    public void ClearChilds() => _childs.Clear();

    public static implicit operator WebElementResponse(WebElement element) =>
        new WebElementResponse(element.Id, element.OuterHTML, element.InnerText);
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
