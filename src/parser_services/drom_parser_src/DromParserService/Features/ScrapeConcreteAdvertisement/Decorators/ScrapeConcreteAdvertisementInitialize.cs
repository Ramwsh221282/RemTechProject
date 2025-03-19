using PuppeteerSharp;
using RemTechCommon.Utils.OptionPattern;
using SharedParsersLibrary.Models;

namespace DromParserService.Features.ScrapeConcreteAdvertisement.Decorators;

public sealed class ScrapeConcreteAdvertisementInitialize(
    IScrapeConcreteAdvertisementHandler handler,
    ScrapeConcreteAdvertisementContext context
) : IScrapeConcreteAdvertisementHandler
{
    private readonly IScrapeConcreteAdvertisementHandler _handler = handler;
    private readonly ScrapeConcreteAdvertisementContext _context = context;

    public async Task<Option<ScrapedAdvertisement>> Handle(
        ScrapeConcreteAdvertisementCommand command
    )
    {
        _context.Page = Option<IPage>.Some(command.Page);
        _context.Advertisement = Option<ScrapedAdvertisement>.Some(command.Advertisement);
        return await _handler.Handle(command);
    }
}
