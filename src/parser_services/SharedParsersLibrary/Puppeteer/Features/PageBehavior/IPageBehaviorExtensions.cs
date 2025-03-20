using PuppeteerSharp;
using RemTechCommon.Utils.OptionPattern;

namespace SharedParsersLibrary.Puppeteer.Features.PageBehavior;

public static class IPageBehaviorExtensions
{
    public static async Task ScrollBottom(this IPage page)
    {
        var executor = page.CreatePageExecutor();
        var scrollBottom = new ScrollBottomBehavior();
        await executor.Invoke(scrollBottom);
    }

    public static async Task ScrollTop(this IPage page)
    {
        var executor = page.CreatePageExecutor();
        var scrollTop = new ScrollTopBehavior();
        await executor.Invoke(scrollTop);
    }

    public static async Task<Option<IElementHandle>> GetElementWithClassFormatter(
        this IPage page,
        string className
    )
    {
        var executor = page.CreatePageExecutor();
        var getElement = new GetElementWithSelectorFormat(className);
        var result = await executor.Invoke(getElement);
        return result == null ? result.AsNone<IElementHandle>() : result.AsSome<IElementHandle>();
    }

    public static async Task<Option<IElementHandle>> GetElementWithoutClassFormatter(
        this IPage page,
        string className
    )
    {
        var executor = page.CreatePageExecutor();
        var getElement = new GetElementByPage(className);
        var result = await executor.Invoke(getElement);
        return result == null ? result.AsNone<IElementHandle>() : result.AsSome<IElementHandle>();
    }

    public static async Task<Option<IElementHandle[]>> GetElementsArrayWithClassFormatter(
        this IPage page,
        string className
    )
    {
        var executor = page.CreatePageExecutor();
        var getElements = new GetElementsArrayWithSelectorFormat(className);
        var results = await executor.Invoke(getElements);
        return results.Length == 0
            ? results.AsNone<IElementHandle[]>()
            : results.AsSome<IElementHandle[]>();
    }

    public static PageBehaviorExecutor CreatePageExecutor(this IPage page) =>
        new PageBehaviorExecutor(page);
}
