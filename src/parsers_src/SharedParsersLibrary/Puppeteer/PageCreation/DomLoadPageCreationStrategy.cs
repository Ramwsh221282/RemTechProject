using PuppeteerSharp;

namespace SharedParsersLibrary.Puppeteer.PageCreation;

public sealed class DomLoadPageCreationStrategy : IPageCreationStrategy
{
    private readonly IBrowser _browser;

    public DomLoadPageCreationStrategy(IBrowser browser)
    {
        _browser = browser;
    }

    public async Task<IPage> Create(string pageUrl)
    {
        IPage page = await _browser.NewPageAsync();
        await page.GoToAsync(pageUrl, WaitUntilNavigation.DOMContentLoaded);
        return page;
    }
}
