using PuppeteerSharp;
using RemTech.Shared.SDK.CqrsPattern.Commands;
using RemTech.Shared.SDK.OptionPattern;
using SharedParsersLibrary.Models;
using SharedParsersLibrary.Puppeteer.ElementBehavior;
using SharedParsersLibrary.Puppeteer.PageBehavior;

namespace RemTech.Parser.Avito.Scraping.ScrapeAdvertisement.Decorators;

public sealed class ScrapeConcreteAdvertisementDescriptionDecorator(
    ICommandHandler<ScrapeConcreteAdvertisementCommand, Option<ScrapedAdvertisement>> handler,
    ScrapeConcreteAdvertisementContext context
) : ICommandHandler<ScrapeConcreteAdvertisementCommand, Option<ScrapedAdvertisement>>
{
    private const string selector = "div[data-marker='item-view/item-description']";

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

        Option<IElementHandle> description = await page.GetElementWithoutClassFormatter(selector);
        if (description.HasValue == false)
            return Option<ScrapedAdvertisement>.None();

        Option<string> text = await description.Value.GetElementText();
        if (text.HasValue == false)
            return Option<ScrapedAdvertisement>.None();

        _context.SetDescription(text.Value);
        return await _handler.Handle(command, ct);
    }
}
