using System.Globalization;
using PuppeteerSharp;
using RemTechCommon.Utils.OptionPattern;
using SharedParsersLibrary.Models;
using SharedParsersLibrary.Puppeteer.Features.ElementBehavior;
using SharedParsersLibrary.Puppeteer.Features.PageBehavior;

namespace DromParserService.Features.ScrapeConcreteAdvertisement.Decorators;

public sealed class ScrapeAdvertisementDateDecorator(
    IScrapeConcreteAdvertisementHandler handler,
    ScrapeConcreteAdvertisementContext context
) : IScrapeConcreteAdvertisementHandler
{
    private readonly IScrapeConcreteAdvertisementHandler _handler = handler;
    private readonly ScrapeConcreteAdvertisementContext _context = context;
    private const string _selector = "css-pxeubi evnwjo70";

    public async Task<Option<ScrapedAdvertisement>> Handle(
        ScrapeConcreteAdvertisementCommand command
    )
    {
        IPage page = _context.Page.Value;
        Option<IElementHandle> dateElement = await page.GetElementWithClassFormatter(_selector);
        if (!dateElement.HasValue)
            return await _handler.Handle(command);

        await using IElementHandle dateNode = dateElement.Value;
        Option<string> dateText = await dateNode.GetElementText();
        if (!dateText.HasValue)
            return await _handler.Handle(command);

        string dateTextValue = dateText.Value;
        string[] splitted = dateTextValue.Split(' ', StringSplitOptions.TrimEntries);
        string dateString = splitted[^1];

        DateTime parsedDate = DateTime.ParseExact(
            dateString,
            "dd.MM.yyyy",
            CultureInfo.InvariantCulture
        );
        _context.Advertisement.Value.Published = parsedDate;
        return await _handler.Handle(command);
    }
}
