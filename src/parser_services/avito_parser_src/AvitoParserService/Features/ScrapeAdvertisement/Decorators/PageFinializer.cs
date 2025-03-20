using AvitoParserService.Common.Cqrs;
using RemTechCommon.Utils.OptionPattern;
using SharedParsersLibrary.Models;

namespace AvitoParserService.Features.ScrapeAdvertisement.Decorators;

public sealed class PageFinializer
    : ICommandHandler<ScrapeConcreteAdvertisementCommand, Option<ScrapedAdvertisement>>
{
    private readonly ICommandHandler<
        ScrapeConcreteAdvertisementCommand,
        Option<ScrapedAdvertisement>
    > _handler;

    public PageFinializer(
        ICommandHandler<ScrapeConcreteAdvertisementCommand, Option<ScrapedAdvertisement>> handler
    ) => _handler = handler;

    public async Task<Option<ScrapedAdvertisement>> Handle(
        ScrapeConcreteAdvertisementCommand command
    )
    {
        Option<ScrapedAdvertisement> advertisement = await _handler.Handle(command);
        await command.Page.DisposeAsync();
        return advertisement;
    }
}
