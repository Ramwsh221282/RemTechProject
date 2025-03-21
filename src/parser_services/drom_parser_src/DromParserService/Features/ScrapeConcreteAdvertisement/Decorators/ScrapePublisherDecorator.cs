using PuppeteerSharp;
using RemTechCommon.Utils.OptionPattern;
using SharedParsersLibrary.Models;
using SharedParsersLibrary.Puppeteer.Features.ElementBehavior;
using SharedParsersLibrary.Puppeteer.Features.PageBehavior;

namespace DromParserService.Features.ScrapeConcreteAdvertisement.Decorators;

public sealed class ScrapePublisherDecorator(
    IScrapeConcreteAdvertisementHandler handler,
    ScrapeConcreteAdvertisementContext context,
    Serilog.ILogger logger
) : IScrapeConcreteAdvertisementHandler
{
    private readonly Serilog.ILogger _logger = logger;
    private readonly IScrapeConcreteAdvertisementHandler _handler = handler;
    private readonly ScrapeConcreteAdvertisementContext _context = context;
    const string _companySelector = "a[data-ga-stats-name='dealer_name']";
    const string _personSelector = "div[data-ga-stats-name='private_person']";

    public async Task<Option<ScrapedAdvertisement>> Handle(
        ScrapeConcreteAdvertisementCommand command
    )
    {
        IPage page = _context.Page.Value;
        Option<IElementHandle> companyInfo = await page.GetElementWithoutClassFormatter(
            _companySelector
        );
        if (companyInfo.HasValue)
        {
            await using IElementHandle element = companyInfo.Value;
            Option<string> text = await element.GetElementText();
            if (text.HasValue)
            {
                _context.Advertisement.Value.Publisher = text.Value;
                return await _handler.Handle(command);
            }
        }
        Option<IElementHandle> personInfo = await page.GetElementWithoutClassFormatter(
            _personSelector
        );
        if (personInfo.HasValue)
        {
            await using IElementHandle element = personInfo.Value;
            Option<IElementHandle> personNode = await element.GetChildWithClassFormatter(
                "_1n15liu7"
            );
            if (personNode.HasValue)
            {
                await using IElementHandle personNodeElement = personNode.Value;
                Option<string> text = await personNodeElement.GetElementText();
                if (text.HasValue)
                {
                    _context.Advertisement.Value.Publisher = text.Value;
                    return await _handler.Handle(command);
                }
            }
        }
        _logger.Error(
            "{Context} publisher not found. {Url}",
            nameof(ScrapePublisherDecorator),
            _context.Advertisement.Value.SourceUrl
        );
        return Option<ScrapedAdvertisement>.None();
    }
}
