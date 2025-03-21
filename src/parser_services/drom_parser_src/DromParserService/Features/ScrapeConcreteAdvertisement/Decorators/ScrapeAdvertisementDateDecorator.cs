using System.Globalization;
using PuppeteerSharp;
using RemTechCommon.Utils.OptionPattern;
using SharedParsersLibrary.Models;
using SharedParsersLibrary.Puppeteer.Features.ElementBehavior;
using SharedParsersLibrary.Puppeteer.Features.PageBehavior;

namespace DromParserService.Features.ScrapeConcreteAdvertisement.Decorators;

public sealed class ScrapeAdvertisementDateDecorator(
    IScrapeConcreteAdvertisementHandler handler,
    ScrapeConcreteAdvertisementContext context,
    Serilog.ILogger logger
) : IScrapeConcreteAdvertisementHandler
{
    private readonly Serilog.ILogger _logger = logger;
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
        {
            _logger.Error(
                "{Context} no date found. Url: {Url}",
                nameof(ScrapeAdvertisementDateDecorator),
                _context.Advertisement.Value.SourceUrl
            );
            return Option<ScrapedAdvertisement>.None();
        }

        await using IElementHandle dateNode = dateElement.Value;
        Option<string> dateText = await dateNode.GetElementText();
        if (!dateText.HasValue)
        {
            _logger.Error(
                "{Context} no date found. Url: {Url}",
                nameof(ScrapeAdvertisementDateDecorator),
                _context.Advertisement.Value.SourceUrl
            );
            return Option<ScrapedAdvertisement>.None();
        }

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
