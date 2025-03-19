using PuppeteerSharp;

namespace SharedParsersLibrary.Puppeteer.Features.PageCreation;

public sealed class FullLoadPageCreationStrategy : IPageCreationStrategy
{
    private readonly IBrowser _browser;

    public FullLoadPageCreationStrategy(IBrowser browser) => _browser = browser;

    public async Task<IPage> Create(string pageUrl)
    {
        IPage page = await _browser.NewPageAsync();
        await page.GoToAsync(pageUrl, WaitUntilNavigation.Load);
        return page;
    }
}
