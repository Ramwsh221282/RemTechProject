using PuppeteerSharp;
using RemTechCommon.Utils.OptionPattern;
using SharedParsersLibrary.Contracts;
using SharedParsersLibrary.Models;
using SharedParsersLibrary.Puppeteer.Features.BrowserCreation;
using ILogger = Serilog.ILogger;

namespace AvitoParserService.Features.ScrapeCatalogue.Decorators;

public sealed class ScrapeCatalogueInstallBrowser : IScrapeAdvertisementsHandler
{
    private readonly ScrapeCatalogueContext _context;
    private readonly BrowserFactory _factory;
    private readonly ILogger _logger;
    private readonly IScrapeAdvertisementsHandler _handler;

    public ScrapeCatalogueInstallBrowser(
        ScrapeCatalogueContext context,
        IScrapeAdvertisementsHandler handler,
        BrowserFactory factory,
        ILogger logger
    )
    {
        _context = context;
        _factory = factory;
        _logger = logger;
        _handler = handler;
    }

    public async Task Handle(ScrapeAdvertisementCommand command)
    {
        _logger.Information(
            "{Context} checking browser executable...",
            nameof(ScrapedAdvertisement)
        );
        bool isInstalled = await _factory.LoadPuppeteerIfNotExists();
        if (!isInstalled)
            _logger.Information(
                "{Context} browser is already installed.",
                nameof(ScrapeAdvertisementCommand)
            );
        else
            _logger.Information(
                "{Context} browser has been installed.",
                nameof(ScrapeAdvertisementCommand)
            );

        _logger.Information(
            "{Context} created browser instance.",
            nameof(ScrapeAdvertisementCommand)
        );
        IBrowser browser = await _factory.CreateStealthBrowserInstance();
        _context.Browser = Option<IBrowser>.Some(browser);
        await _handler.Handle(command);
    }
}
