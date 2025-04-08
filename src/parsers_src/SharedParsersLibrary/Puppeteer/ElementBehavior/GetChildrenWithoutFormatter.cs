using PuppeteerSharp;

namespace SharedParsersLibrary.Puppeteer.ElementBehavior;

public sealed class GetChildrenWithoutFormatter(string className)
    : IElementBehavior<IElementHandle[]>
{
    private readonly string _className = className;

    public async Task<IElementHandle[]> Invoke(IElementHandle element)
    {
        try
        {
            IElementHandle[] elements = await element.QuerySelectorAllAsync(_className);
            return elements;
        }
        catch
        {
            return [];
        }
    }
}
