using DromParserService.Features.DromCatalogueScraping;
using DromParserService.Features.DromCatalogueScraping.CatalogueScraping;
using SharedParsersLibrary.Contracts;
using SharedParsersLibrary.Sinking;

namespace DromScrapingTests.DromCatalogueScrapingModels;

public sealed class DromScrapeAdvertisementsByJsonCoreHandler(
    DromCatalogueScrapingContext context,
    SinkerSenderFactory factory,
    IServiceProvider provider
) : IScrapeAdvertisementsHandler
{
    private readonly DromCatalogueScrapingContext _context = context;
    private readonly IServiceProvider _provider = provider;
    private readonly SinkerSenderFactory _factory = factory;
    private const int MaxDegreeOfParallelism = 5;

    public async Task Handle(ScrapeAdvertisementCommand command)
    {
        _context.Dispose();
        ParallelConcreteAdvertisementScraper concreteAdvertisementScraper = new(
            _factory,
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
