using RemTech.Parser.Drom.Scraping.ConcreteAdvertisementScraping;
using SharedParsersLibrary.Contracts;

namespace RemTech.Parser.Drom.Scraping.CatalogueScraping;

public sealed class DromScrapeCatalogueHandler(
    IServiceProvider provider,
    DromCatalogueScrapingContext context
) : IScrapeAdvertisementsHandler
{
    private readonly DromCatalogueScrapingContext _context = context;
    private readonly IServiceProvider _provider = provider;
    private const int MaxDegreeOfParallelism = 5;

    public async Task Handle(ScrapeAdvertisementsCommand command)
    {
        _context.Dispose();
        ParallelConcreteAdvertisementScraper concreteAdvertisementScraper = new(
            "DROM",
            MaxDegreeOfParallelism,
            3,
            _provider
        );
        await concreteAdvertisementScraper.ExecuteScrape(_context.EnumerateAdvertisements());
        while (
            concreteAdvertisementScraper is { FailedResultsCount: > 0, IsReachedMaxCount: false }
        )
        {
            await concreteAdvertisementScraper.ExecuteScrape();
        }
    }
}
