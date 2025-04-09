using RemTech.Parser.Drom.Scraping.CatalogueScraping.Decorators;
using RemTech.Parser.Drom.Scraping.ConcreteAdvertisementScraping;
using SharedParsersLibrary.Contracts;

namespace RemTech.Parser.Drom.Scraping.CatalogueScraping;

public sealed class DromCatalogueScraperFactory(
    ConcreteAdvertisementScraperFactory factory,
    Serilog.ILogger logger
) : IScrapeAdvertisementHandlerFactory
{
    private readonly ConcreteAdvertisementScraperFactory _factory = factory;
    private readonly Serilog.ILogger _logger = logger;

    public IScrapeAdvertisementsHandler Create()
    {
        DromCatalogueScrapingContext context = new();
        DromScrapeCatalogueHandler handler = new(context, factory);
        DromScrapeCataloguePagesDecorator pagesHandler = new(handler, context, _logger);
        DromScrapeCatalogueInitializeDecorator initializer = new(pagesHandler, context);
        return initializer;
    }
}
