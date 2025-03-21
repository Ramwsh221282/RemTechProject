using PuppeteerSharp;
using RemTechCommon.Utils.OptionPattern;
using SharedParsersLibrary.Models;

namespace DromParserService.Features.ScrapeConcreteAdvertisement;

public sealed record ScrapeConcreteAdvertisementCommand(
    IPage Page,
    ScrapedAdvertisement Advertisement
);

public sealed class ScrapeConcreteAdvertisementCommandHandler(
    ScrapeConcreteAdvertisementContext context
) : IScrapeConcreteAdvertisementHandler
{
    private readonly ScrapeConcreteAdvertisementContext _context = context;

    public async Task<Option<ScrapedAdvertisement>> Handle(
        ScrapeConcreteAdvertisementCommand command
    )
    {
        return await Task.FromResult(_context.Advertisement);
    }
}
