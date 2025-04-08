using PuppeteerSharp;

namespace SharedParsersLibrary.Puppeteer.PageCreation;

public sealed class NetWorkIdle0PageCreationStrategy : IPageCreationStrategy
{
    private readonly IBrowser _browser;

    public NetWorkIdle0PageCreationStrategy(IBrowser browser)
    {
        _browser = browser;
    }

    public async Task<IPage> Create(string pageUrl)
    {
        IPage page = await _browser.NewPageAsync();
        await page.GoToAsync(pageUrl, WaitUntilNavigation.Networkidle0);
        return page;
    }
}
