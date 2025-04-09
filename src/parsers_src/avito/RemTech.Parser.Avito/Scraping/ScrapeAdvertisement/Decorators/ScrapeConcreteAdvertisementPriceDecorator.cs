using PuppeteerSharp;
using RemTech.Shared.SDK.CqrsPattern.Commands;
using RemTech.Shared.SDK.OptionPattern;
using SharedParsersLibrary.Models;
using SharedParsersLibrary.Puppeteer.ElementBehavior;
using SharedParsersLibrary.Puppeteer.PageBehavior;

namespace RemTech.Parser.Avito.Scraping.ScrapeAdvertisement.Decorators;

public sealed class ScrapeConcreteAdvertisementPriceDecorator(
    ICommandHandler<ScrapeConcreteAdvertisementCommand, Option<ScrapedAdvertisement>> handler,
    ScrapeConcreteAdvertisementContext context
) : ICommandHandler<ScrapeConcreteAdvertisementCommand, Option<ScrapedAdvertisement>>
{
    private const string container = "style-price-value-main-TIg6u";
    private const string attribute = "content";
    private const string additional = "style-price-value-additional-pFInr";
    private const string priceSpan = "span[itemprop='price']";

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
        Option<IElementHandle> priceContainer = await page.GetElementWithClassFormatter(container);
        if (priceContainer.HasValue == false)
            return Option<ScrapedAdvertisement>.None();

        Option<IElementHandle> priceNode = await priceContainer.Value.GetChildWithoutClassFormatter(
            priceSpan
        );
        if (priceNode.HasValue == false)
            return Option<ScrapedAdvertisement>.None();

        Option<string> priceValue = await priceNode.Value.GetElementAttributeValue(attribute);
        if (priceValue.HasValue == false || !long.TryParse(priceValue.Value, out long price))
            return Option<ScrapedAdvertisement>.None();

        _context.SetPrice(price);

        Option<IElementHandle> priceAdditionalContainer = await page.GetElementWithClassFormatter(
            additional
        );
        if (priceAdditionalContainer.HasValue == false)
            return await _handler.Handle(command, ct);

        Option<string> additionalText = await priceAdditionalContainer.Value.GetElementText();
        if (additionalText.HasValue == false)
            return await _handler.Handle(command, ct);

        _context.SetPriceExtra(additionalText.Value);
        return await _handler.Handle(command, ct);
    }
}
