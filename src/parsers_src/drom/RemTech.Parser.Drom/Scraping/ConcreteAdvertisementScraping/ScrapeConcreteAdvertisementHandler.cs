using RemTech.Parser.Drom.Scraping.Models;
using RemTech.Shared.SDK.OptionPattern;
using SharedParsersLibrary.Models;

namespace RemTech.Parser.Drom.Scraping.ConcreteAdvertisementScraping;

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
        fromCatalogue = fromCatalogue with { PriceExtra = _context.PriceExtra };
        _context.DisposeDocument();
        return await Task.FromResult(fromCatalogue);
    }
}
