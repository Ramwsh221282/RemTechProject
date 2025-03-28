using PuppeteerSharp;
using RemTechCommon.Utils.Extensions;
using RemTechCommon.Utils.OptionPattern;
using SharedParsersLibrary.Models;
using SharedParsersLibrary.Puppeteer.Features.ElementBehavior;
using SharedParsersLibrary.Puppeteer.Features.PageBehavior;

namespace DromParserService.Features.ScrapeConcreteAdvertisement.Decorators;

public sealed class ScrapeAdvertisementDescriptionDecorator(
    IScrapeConcreteAdvertisementHandler handler,
    ScrapeConcreteAdvertisementContext context,
    Serilog.ILogger logger
) : IScrapeConcreteAdvertisementHandler
{
    private readonly Serilog.ILogger _logger = logger;
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
        {
            _logger.Error(
                "{Context} no description found. Url: {Url}",
                nameof(ScrapeAdvertisementDateDecorator),
                _context.Advertisement.Value.SourceUrl
            );
            return Option<ScrapedAdvertisement>.None();
        }

        await using IElementHandle description = descriptionElement.Value;
        Option<string> text = await description.GetElementText();
        if (!text.HasValue)
        {
            _logger.Error(
                "{Context} no description found. Url: {Url}",
                nameof(ScrapeAdvertisementDateDecorator),
                _context.Advertisement.Value.SourceUrl
            );
            return Option<ScrapedAdvertisement>.None();
        }

        _context.Advertisement.Value.Description = text
            .Value.Replace("Дополнительно: ", "")
            .CleanString();
        return await _handler.Handle(command);
    }
}
