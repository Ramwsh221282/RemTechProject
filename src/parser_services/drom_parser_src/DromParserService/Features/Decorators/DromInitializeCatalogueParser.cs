using PuppeteerSharp;
using RemTechCommon.Utils.OptionPattern;
using SharedParsersLibrary.Contracts;
using SharedParsersLibrary.Puppeteer.Features.BrowserCreation;

namespace DromParserService.Features.Decorators;

public sealed class DromInitializeCatalogueParser(
    IScrapeAdvertisementsHandler handler,
    DromCatalogueScrapingContext context
) : IScrapeAdvertisementsHandler
{
    private readonly DromCatalogueScrapingContext _context = context;
    private readonly IScrapeAdvertisementsHandler _handler = handler;
    private readonly BrowserFactory _factory = new BrowserFactory();

    public async Task Handle(ScrapeAdvertisementCommand command)
    {
        await _factory.LoadPuppeteerIfNotExists();
        IBrowser browser = await _factory.CreateStealthBrowserInstance();
        _context.Browser = Option<IBrowser>.Some(browser);
        await _handler.Handle(command);
    }
}
