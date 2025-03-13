using AvitoParser.PDK.Models.ValueObjects;
using PuppeteerSharp;

namespace CollectAdvertisementsDataFromCataloguePlugin.Models;

public sealed record DromCataloguePage : IDisposable, IAsyncDisposable
{
    public IPage Page { get; }

    private DromCataloguePage(IPage page) => Page = page;

    public void Dispose() => Page.Dispose();

    public async ValueTask DisposeAsync() => await Page.DisposeAsync();

    public static async Task<DromCataloguePage> Create(IBrowser browser, ScrapedSourceUrl url)
    {
        IPage page = await browser.NewPageAsync();
        await page.GoToAsync(url.SourceUrl, WaitUntilNavigation.DOMContentLoaded);
        return new DromCataloguePage(page);
    }
}
