using PuppeteerSharp;

namespace SharedParsersLibrary.Puppeteer.Features.PageBehavior;

public sealed class GetElementsArrayWithSelectorFormat : IPageBehavior<IElementHandle[]>
{
    private readonly ClassNameFormatter _formatter;

    public GetElementsArrayWithSelectorFormat(string cssSelector)
    {
        _formatter = new ClassNameFormatter(cssSelector);
    }

    public async Task<IElementHandle[]> Invoke(IPage page)
    {
        string formatted = _formatter.MakeFormatted();
        try
        {
            await page.WaitForSelectorAsync(formatted);
            return await page.QuerySelectorAllAsync(formatted);
        }
        catch
        {
            return [];
        }
    }
}
