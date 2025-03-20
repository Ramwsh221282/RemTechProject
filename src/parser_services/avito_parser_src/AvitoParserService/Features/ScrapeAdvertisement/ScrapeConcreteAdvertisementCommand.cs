using AvitoParserService.Common.Cqrs;
using PuppeteerSharp;
using RemTechCommon.Utils.OptionPattern;
using SharedParsersLibrary.Models;

namespace AvitoParserService.Features.ScrapeAdvertisement;

public sealed record ScrapeConcreteAdvertisementCommand(
    IPage Page,
    ScrapedAdvertisement Advertisement
) : ICommand<Option<ScrapedAdvertisement>>;

public sealed class ScrapeConcreteAdvertisementCommandHandler
    : ICommandHandler<ScrapeConcreteAdvertisementCommand, Option<ScrapedAdvertisement>>
{
    private readonly ScrapeConcreteAdvertisementContext _context;

    public ScrapeConcreteAdvertisementCommandHandler(ScrapeConcreteAdvertisementContext context) =>
        _context = context;

    public async Task<Option<ScrapedAdvertisement>> Handle(
        ScrapeConcreteAdvertisementCommand command
    )
    {
        return _context.ScrapedAdvertisement;
    }
}
