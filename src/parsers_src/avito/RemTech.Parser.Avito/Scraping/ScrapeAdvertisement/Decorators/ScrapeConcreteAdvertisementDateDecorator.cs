using PuppeteerSharp;
using RemTech.Parser.Avito.Scraping.Converters.AvitoDateConverting;
using RemTech.Shared.SDK.CqrsPattern.Commands;
using RemTech.Shared.SDK.OptionPattern;
using RemTech.Shared.SDK.ResultPattern;
using SharedParsersLibrary.Models;
using SharedParsersLibrary.Puppeteer.ElementBehavior;
using SharedParsersLibrary.Puppeteer.PageBehavior;

namespace RemTech.Parser.Avito.Scraping.ScrapeAdvertisement.Decorators;

public sealed class ScrapeConcreteAdvertisementDateDecorator(
    ICommandHandler<ScrapeConcreteAdvertisementCommand, Option<ScrapedAdvertisement>> handler,
    ScrapeConcreteAdvertisementContext context
) : ICommandHandler<ScrapeConcreteAdvertisementCommand, Option<ScrapedAdvertisement>>
{
    private const string selector = "span[data-marker='item-view/item-date']";

    private readonly ICommandHandler<
        ScrapeConcreteAdvertisementCommand,
        Option<ScrapedAdvertisement>
    > _handler = handler;

    private readonly ScrapeConcreteAdvertisementContext _context = context;

    public async Task<Option<ScrapedAdvertisement>> Handle(
        ScrapeConcreteAdvertisementCommand command,
        CancellationToken ct = default
    )
    {
        IPage page = command.Page;

        Option<IElementHandle> element = await page.GetElementWithoutClassFormatter(selector);
        if (element.HasValue == false)
            return Option<ScrapedAdvertisement>.None();

        Option<string> text = await element.Value.GetElementText();
        if (text.HasValue == false)
            return Option<ScrapedAdvertisement>.None();

        IAvitoDateConverter converter = new CompositeDateConverter(text.Value);
        Result<DateTime> date = converter.Convert();
        if (date.IsFailure)
            return Option<ScrapedAdvertisement>.None();

        _context.SetDate(date.Value);
        return await _handler.Handle(command, ct);
    }
}
