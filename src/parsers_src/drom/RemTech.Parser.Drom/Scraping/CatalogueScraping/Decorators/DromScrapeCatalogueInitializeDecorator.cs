using PuppeteerSharp;
using RemTech.Shared.SDK.OptionPattern;
using SharedParsersLibrary.Contracts;
using SharedParsersLibrary.Puppeteer.BrowserCreation;

namespace RemTech.Parser.Drom.Scraping.CatalogueScraping.Decorators;

public sealed class DromScrapeCatalogueInitializeDecorator(
    IScrapeAdvertisementsHandler handler,
    DromCatalogueScrapingContext context
) : IScrapeAdvertisementsHandler
{
    private readonly DromCatalogueScrapingContext _context = context;
    private readonly IScrapeAdvertisementsHandler _handler = handler;

    public async Task Handle(ScrapeAdvertisementsCommand command)
    {
        IBrowser browser = await BrowserFactory.CreateStealthBrowserInstance(headless: true);
        _context.Browser = Option<IBrowser>.Some(browser);

        try
        {
            await _handler.Handle(command);
        }
        catch
        {
            _context.Dispose();
        }
    }
}
