using RemTech.Shared.SDK.CqrsPattern.Commands;
using RemTech.Shared.SDK.OptionPattern;
using SharedParsersLibrary.Models;

namespace RemTech.Parser.Avito.Scraping.ScrapeAdvertisement;

public sealed class ScrapeConcreteAdvertisementCommandHandler(
    ScrapeConcreteAdvertisementContext context
) : ICommandHandler<ScrapeConcreteAdvertisementCommand, Option<ScrapedAdvertisement>>
{
    private readonly ScrapeConcreteAdvertisementContext _context = context;

    public async Task<Option<ScrapedAdvertisement>> Handle(
        ScrapeConcreteAdvertisementCommand command,
        CancellationToken ct = default
    ) => await Task.FromResult(_context.ScrapedAdvertisement);
}
