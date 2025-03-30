using DromParserService.Features.DromCatalogueScraping.Models;
using RemTechCommon.Utils.OptionPattern;
using SharedParsersLibrary.Models;

namespace DromParserService.Features.DromCatalogueScraping.ConcreteAdvertisementScraping;

public sealed class ScrapeConcreteAdvertisementHandler(ScrapeConcreteAdvertisementContext context)
    : IScrapeConcreteAdvertisementHandler
{
    private readonly ScrapeConcreteAdvertisementContext _context = context;

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
