using PuppeteerSharp;
using RemTechCommon.Utils.OptionPattern;
using SharedParsersLibrary.Models;
using SharedParsersLibrary.Puppeteer.Features.ElementBehavior;
using SharedParsersLibrary.Puppeteer.Features.PageBehavior;

namespace DromParserService.Features.ScrapeConcreteAdvertisement.Decorators;

public sealed class ScrapePublisherDecorator(
    IScrapeConcreteAdvertisementHandler handler,
    ScrapeConcreteAdvertisementContext context
) : IScrapeConcreteAdvertisementHandler
{
    private readonly IScrapeConcreteAdvertisementHandler _handler = handler;
    private readonly ScrapeConcreteAdvertisementContext _context = context;

    const string _selector = "a[data-ga-stats-name='dealer-name']";

    public async Task<Option<ScrapedAdvertisement>> Handle(
        ScrapeConcreteAdvertisementCommand command
    )
    {
        IPage page = _context.Page.Value;
        var publisherInfoElement = await page.GetElementWithoutClassFormatter(_selector);
        if (!publisherInfoElement.HasValue)
            return await _handler.Handle(command);

        await using IElementHandle publisherElement = publisherInfoElement.Value;
        Option<string> text = await publisherElement.GetElementText();
        if (!text.HasValue)
            return await _handler.Handle(command);

        string textValue = text.Value;
        _context.Advertisement.Value.Publisher = textValue;
        return await _handler.Handle(command);
    }
}
