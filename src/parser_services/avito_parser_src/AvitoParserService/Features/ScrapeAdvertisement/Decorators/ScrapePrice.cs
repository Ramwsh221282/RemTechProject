using AvitoParserService.Common.Cqrs;
using PuppeteerSharp;
using RemTechCommon.Utils.OptionPattern;
using SharedParsersLibrary.Models;
using SharedParsersLibrary.Puppeteer.Features.ElementBehavior;
using SharedParsersLibrary.Puppeteer.Features.PageBehavior;

namespace AvitoParserService.Features.ScrapeAdvertisement.Decorators;

public sealed class ScrapePrice
    : ICommandHandler<ScrapeConcreteAdvertisementCommand, Option<ScrapedAdvertisement>>
{
    private const string container = "style-price-value-main-TIg6u";
    private const string attribute = "content";
    private const string additional = "style-price-value-additional-pFInr";
    private const string priceSpan = "span[itemprop='price']";

    private readonly ICommandHandler<
        ScrapeConcreteAdvertisementCommand,
        Option<ScrapedAdvertisement>
    > _handler;

    private readonly ScrapeConcreteAdvertisementContext _context;

    public ScrapePrice(
        ICommandHandler<ScrapeConcreteAdvertisementCommand, Option<ScrapedAdvertisement>> handler,
        ScrapeConcreteAdvertisementContext context
    )
    {
        _handler = handler;
        _context = context;
    }

    public async Task<Option<ScrapedAdvertisement>> Handle(
        ScrapeConcreteAdvertisementCommand command
    )
    {
        try
        {
            IPage page = command.Page;
            PageBehaviorExecutor pageExecutor = new PageBehaviorExecutor(page);
            await using IElementHandle? priceContainer = await pageExecutor.Invoke(
                new GetElementWithSelectorFormat(container)
            );
            if (priceContainer == null)
                return Option<ScrapedAdvertisement>.None();

            ElementBehaviorExecutor elementExecutor = new ElementBehaviorExecutor(priceContainer);
            await using IElementHandle? priceNode = await elementExecutor.Invoke(
                new GetChildBehavior(priceSpan)
            );
            if (priceNode == null)
                return Option<ScrapedAdvertisement>.None();

            elementExecutor.SwapElement(priceNode);
            string? priceValue = await elementExecutor.Invoke(
                new GetElementAttributeBehavior(attribute)
            );
            if (string.IsNullOrWhiteSpace(priceValue))
                return Option<ScrapedAdvertisement>.None();

            bool canParse = long.TryParse(priceValue, out long price);
            if (!canParse)
                return Option<ScrapedAdvertisement>.None();
            _context.SetPrice(price);

            await using IElementHandle? priceAdditionalContainer = await pageExecutor.Invoke(
                new GetElementWithSelectorFormat("style-price-value-string-rWMtx")
            );
            if (priceAdditionalContainer == null)
                return await _handler.Handle(command);

            elementExecutor.SwapElement(priceAdditionalContainer);
            await using IElementHandle? priceChild = await elementExecutor.Invoke(
                new GetChildWithFormatterBehavior(additional)
            );
            if (priceChild == null)
                return await _handler.Handle(command);

            elementExecutor.SwapElement(priceChild);
            string? additionalText = await elementExecutor.Invoke(new GetElementTextBehavior());
            if (string.IsNullOrWhiteSpace(additionalText))
                return await _handler.Handle(command);

            _context.SetPriceExtra(additionalText);
            return await _handler.Handle(command);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception at price scraping");
            return Option<ScrapedAdvertisement>.None();
        }
    }
}
