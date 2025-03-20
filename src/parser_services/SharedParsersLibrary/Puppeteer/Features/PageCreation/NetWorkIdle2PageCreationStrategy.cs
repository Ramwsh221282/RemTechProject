using PuppeteerSharp;

namespace SharedParsersLibrary.Puppeteer.Features.PageCreation;

public sealed class NetWorkIdle2PageCreationStrategy : IPageCreationStrategy
{
    private readonly IBrowser _browser;

    public NetWorkIdle2PageCreationStrategy(IBrowser browser)
    {
        _browser = browser;
    }

    public async Task<IPage> Create(string pageUrl)
    {
        IPage page = await _browser.NewPageAsync();
        await page.GoToAsync(pageUrl, WaitUntilNavigation.Networkidle2);
        return page;
    }
}
