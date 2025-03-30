using PuppeteerSharp;
using RemTechCommon.Utils.OptionPattern;
using SharedParsersLibrary.Contracts;
using SharedParsersLibrary.Puppeteer.Features.BrowserCreation;

namespace DromParserService.Features.DromCatalogueScraping.CatalogueScraping.Decorators;

public sealed class DromScrapeCatalogueInitializeDecorator(
    IScrapeAdvertisementsHandler handler,
    DromCatalogueScrapingContext context
) : IScrapeAdvertisementsHandler
{
    private readonly DromCatalogueScrapingContext _context = context;
    private readonly IScrapeAdvertisementsHandler _handler = handler;
    private readonly BrowserFactory _factory = new();

    public async Task Handle(ScrapeAdvertisementCommand command)
    {
        await _factory.LoadPuppeteerIfNotExists();
        IBrowser browser = await _factory.CreateStealthBrowserInstance(headless: true);
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
