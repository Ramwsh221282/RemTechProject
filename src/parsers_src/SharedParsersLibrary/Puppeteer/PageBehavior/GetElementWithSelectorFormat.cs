using PuppeteerSharp;

namespace SharedParsersLibrary.Puppeteer.PageBehavior;

public sealed class GetElementWithSelectorFormat : IPageBehavior<IElementHandle?>
{
    private readonly ClassNameFormatter _formatter;

    public GetElementWithSelectorFormat(string cssSelector)
    {
        _formatter = new ClassNameFormatter(cssSelector);
    }

    public async Task<IElementHandle?> Invoke(IPage page)
    {
        string formatted = _formatter.MakeFormatted();
        try
        {
            await page.WaitForSelectorAsync(formatted);
            IElementHandle? element = await page.QuerySelectorAsync(formatted);
            return element;
        }
        catch
        {
            return null;
        }
    }
}
