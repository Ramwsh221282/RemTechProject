using PuppeteerSharp;
using RemTechCommon.Utils.OptionPattern;
using SharedParsersLibrary.Models;
using SharedParsersLibrary.Puppeteer.Features.ElementBehavior;
using SharedParsersLibrary.Puppeteer.Features.PageBehavior;

namespace DromParserService.Features.ScrapeConcreteAdvertisement.Decorators;

public sealed class ScrapeAdvertisementDescriptionDecorator(
    IScrapeConcreteAdvertisementHandler handler,
    ScrapeConcreteAdvertisementContext context
) : IScrapeConcreteAdvertisementHandler
{
    private readonly IScrapeConcreteAdvertisementHandler _handler = handler;
    private readonly ScrapeConcreteAdvertisementContext _context = context;

    private const string _selector = "css-inmjwf e162wx9x0";

    public async Task<Option<ScrapedAdvertisement>> Handle(
        ScrapeConcreteAdvertisementCommand command
    )
    {
        IPage page = _context.Page.Value;
        Option<IElementHandle> descriptionElement = await page.GetElementWithClassFormatter(
            _selector
        );
        if (!descriptionElement.HasValue)
            return await _handler.Handle(command);

        await using IElementHandle description = descriptionElement.Value;
        Option<string> text = await description.GetElementText();
        if (!text.HasValue)
            return await _handler.Handle(command);

        _context.Advertisement.Value.Description = text.Value;
        return await _handler.Handle(command);
    }
}
