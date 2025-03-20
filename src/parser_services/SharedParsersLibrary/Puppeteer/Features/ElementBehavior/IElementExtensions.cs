using PuppeteerSharp;
using RemTechCommon.Utils.OptionPattern;

namespace SharedParsersLibrary.Puppeteer.Features.ElementBehavior;

public static class IElementExtensions
{
    public static async Task<Option<IElementHandle>> GetChildWithClassFormatter(
        this IElementHandle element,
        string className
    )
    {
        var executor = element.GetExecutorForElement();
        var result = await executor.Invoke(new GetChildWithFormatterBehavior(className));
        return result == null ? result.AsNone<IElementHandle>() : result.AsSome<IElementHandle>();
    }

    public static async Task<Option<IElementHandle[]>> GetChildrenWithClassFormatter(
        this IElementHandle element,
        string className
    )
    {
        var executor = element.GetExecutorForElement();
        var elements = await executor.Invoke(new GetChildrenWithFormatterBehavior(className));
        return elements.Length == 0
            ? elements.AsNone<IElementHandle[]>()
            : elements.AsSome<IElementHandle[]>();
    }

    public static async Task<Option<IElementHandle>> GetChildWithoutClassFormatter(
        this IElementHandle element,
        string className
    )
    {
        var executor = element.GetExecutorForElement();
        var result = await executor.Invoke(new GetChildBehavior(className));
        return result == null ? result.AsNone<IElementHandle>() : result.AsSome<IElementHandle>();
    }

    public static async Task<Option<IElementHandle[]>> GetChildrenWithoutClassFormatter(
        this IElementHandle element,
        string className
    )
    {
        var executor = element.GetExecutorForElement();
        var result = await executor.Invoke(new GetChildrenWithoutFormatter(className));
        return result.Length == 0
            ? result.AsNone<IElementHandle[]>()
            : result.AsSome<IElementHandle[]>();
    }

    public static async Task<Option<string>> GetElementText(this IElementHandle element)
    {
        var executor = element.GetExecutorForElement();
        var text = await executor.Invoke(new GetElementTextBehavior());
        return string.IsNullOrWhiteSpace(text) ? text.AsNone<string>() : text.AsSome<string>();
    }

    public static async Task<Option<string>> GetElementAttributeValue(
        this IElementHandle element,
        string attributeName
    )
    {
        var executor = element.GetExecutorForElement();
        var value = await executor.Invoke(new GetElementAttributeBehavior(attributeName));
        return string.IsNullOrWhiteSpace(value) ? value.AsNone<string>() : value.AsSome<string>();
    }

    public static ElementBehaviorExecutor GetExecutorForElement(this IElementHandle element) =>
        new ElementBehaviorExecutor(element);
}
