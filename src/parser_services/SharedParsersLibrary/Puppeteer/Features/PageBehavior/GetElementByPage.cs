using PuppeteerSharp;

namespace SharedParsersLibrary.Puppeteer.Features.PageBehavior;

public sealed class GetElementByPage(string selector) : IPageBehavior<IElementHandle?>
{
    private readonly string _selector = selector;

    public async Task<IElementHandle?> Invoke(IPage page)
    {
        try
        {
            await page.WaitForSelectorAsync(_selector);
            return await page.QuerySelectorAsync(_selector);
        }
        catch
        {
            return null;
        }
    }
}
