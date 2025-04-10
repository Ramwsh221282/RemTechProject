using PuppeteerSharp;
using RemTech.Shared.SDK.CqrsPattern.Commands;
using RemTech.Shared.SDK.OptionPattern;
using SharedParsersLibrary.Models;
using SharedParsersLibrary.Puppeteer.ElementBehavior;
using SharedParsersLibrary.Puppeteer.PageBehavior;

namespace RemTech.Parser.Avito.Scraping.ScrapeAdvertisement.Decorators;

public sealed class ScrapeConcreteAdvertisementPublisherDecorator(
    ICommandHandler<ScrapeConcreteAdvertisementCommand, Option<ScrapedAdvertisement>> handler,
    ScrapeConcreteAdvertisementContext context
) : ICommandHandler<ScrapeConcreteAdvertisementCommand, Option<ScrapedAdvertisement>>
{
    private const string selector = "div[data-marker='seller-info/name']";

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

        Option<IElementHandle> pageElement = await page.GetElementWithoutClassFormatter(selector);
        if (pageElement.HasValue == false)
            return Option<ScrapedAdvertisement>.None();

        Option<string> text = await pageElement.Value.GetElementText();
        if (text.HasValue == false)
            return Option<ScrapedAdvertisement>.None();

        _context.SetPublisher(text.Value);
        return await _handler.Handle(command, ct);
    }
}
