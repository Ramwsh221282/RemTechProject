using PuppeteerSharp;
using SharedParsersLibrary.Puppeteer.Features.PageBehavior;

namespace SharedParsersLibrary.Puppeteer.Features.ElementBehavior;

public sealed class GetChildrenWithFormatterBehavior : IElementBehavior<IElementHandle[]>
{
    private readonly ClassNameFormatter _formatter;

    public GetChildrenWithFormatterBehavior(string className)
    {
        _formatter = new ClassNameFormatter(className);
    }

    public async Task<IElementHandle[]> Invoke(IElementHandle element)
    {
        string formatted = _formatter.MakeFormatted();
        return await element.QuerySelectorAllAsync(formatted);
    }
}
