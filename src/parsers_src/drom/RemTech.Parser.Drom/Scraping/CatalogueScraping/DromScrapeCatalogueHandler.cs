using RemTech.Parser.Drom.Scraping.ConcreteAdvertisementScraping;
using SharedParsersLibrary.Contracts;

namespace RemTech.Parser.Drom.Scraping.CatalogueScraping;

public sealed class DromScrapeCatalogueHandler(
    DromCatalogueScrapingContext context,
    ConcreteAdvertisementScraperFactory factory
) : IScrapeAdvertisementsHandler
{
    private readonly DromCatalogueScrapingContext _context = context;
    private readonly ConcreteAdvertisementScraperFactory _factory = factory;

    public async Task Handle(ScrapeAdvertisementsCommand command)
    {
        _context.Dispose();
        ParallelConcreteAdvertisementScraper concreteAdvertisementScraper = _factory.Create();

        await concreteAdvertisementScraper.ExecuteScrape(_context.EnumerateAdvertisements());

        while (
            concreteAdvertisementScraper is { FailedResultsCount: > 0, IsReachedMaxCount: false }
        )
        {
            await concreteAdvertisementScraper.ExecuteScrape();
        }
    }
}
