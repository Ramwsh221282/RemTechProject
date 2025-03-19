using PuppeteerSharp;

namespace SharedParsersLibrary.Puppeteer.Features.PageCreation;

public sealed class EmptyPageCreationStrategy : IPageCreationStrategy
{
    private readonly IBrowser _browser;

    public EmptyPageCreationStrategy(IBrowser browser)
    {
        _browser = browser;
    }

    public async Task<IPage> Create(string pageUrl)
    {
        IPage page = await _browser.NewPageAsync();
        return page;
    }
}
