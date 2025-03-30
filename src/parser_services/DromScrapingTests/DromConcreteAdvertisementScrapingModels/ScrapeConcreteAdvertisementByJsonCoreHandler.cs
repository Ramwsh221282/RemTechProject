using DromParserService.Features.DromCatalogueScraping.ConcreteAdvertisementScraping;
using DromScrapingTests.DromDeserializationModels;
using RemTechCommon.Utils.OptionPattern;
using SharedParsersLibrary.Models;

namespace DromScrapingTests.DromConcreteAdvertisementScrapingModels;

public sealed class ScrapeConcreteAdvertisementByJsonCoreHandler(
    ScrapeConcreteAdvertisementByJsonContext context
) : IScrapeConcreteAdvertisementHandler
{
    private readonly ScrapeConcreteAdvertisementByJsonContext _context = context;

    public async Task<Option<ScrapedAdvertisement>> Handle(
        ScrapeConcreteAdvertisementCommand command
    )
    {
        if (!_context.ScriptElement.HasValue)
            return Option<ScrapedAdvertisement>.None();
        DromScrapedJsonAttributesInitializer initializer = new(_context.ScriptElement.Value);
        ScrapedAdvertisement fromCatalogue = command.Advertisement;
        fromCatalogue = initializer.Set(fromCatalogue);
        fromCatalogue.PriceExtra = _context.PriceExtra;
        _context.DisposeDocument();
        return await Task.FromResult(fromCatalogue);
    }
}
