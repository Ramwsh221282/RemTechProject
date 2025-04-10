using PuppeteerSharp;

namespace SharedParsersLibrary.Puppeteer.PageCreation;

public sealed class DomLoadPageCreationStrategyNoImages(IBrowser browser) : IPageCreationStrategy
{
    private readonly IBrowser _browser = browser;

    public async Task<IPage> Create(string pageUrl)
    {
        IPage page = await _browser.NewPageAsync();
        await page.SetRequestInterceptionAsync(true);
        page.Request += async (_, e) =>
        {
            if (e.Request.ResourceType is ResourceType.Image or ResourceType.StyleSheet)
                await e.Request.AbortAsync();
            else
                await e.Request.ContinueAsync();
        };
        await page.GoToAsync(pageUrl, WaitUntilNavigation.DOMContentLoaded);
        return page;
    }
}
