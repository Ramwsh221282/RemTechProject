using PuppeteerSharp;
using RemTechCommon.Utils.OptionPattern;
using SharedParsersLibrary.Models;
using SharedParsersLibrary.Puppeteer.Features.ElementBehavior;
using SharedParsersLibrary.Puppeteer.Features.PageBehavior;

namespace DromParserService.Features.ScrapeConcreteAdvertisement.Decorators;

public sealed class ScrapeAdvertisementPriceDecorator(
    IScrapeConcreteAdvertisementHandler handler,
    ScrapeConcreteAdvertisementContext context,
    Serilog.ILogger logger
) : IScrapeConcreteAdvertisementHandler
{
    private readonly Serilog.ILogger _logger = logger;
    private readonly IScrapeConcreteAdvertisementHandler _handler = handler;
    private readonly ScrapeConcreteAdvertisementContext _context = context;
    private const string containerSelector = "css-pmcte9 e1ab30xp0";
    private const string priceNodeSelector = "wb9m8q0";
    private const string priceExtraSelector = "css-1oit28e ejipaoe0";

    public async Task<Option<ScrapedAdvertisement>> Handle(
        ScrapeConcreteAdvertisementCommand command
    )
    {
        IPage page = _context.Page.Value;
        Option<IElementHandle> priceContainer = await page.GetElementWithClassFormatter(
            containerSelector
        );
        if (!priceContainer.HasValue)
        {
            _logger.Error(
                "{Context} no price found. Url: {Url}",
                nameof(ScrapeAdvertisementPriceDecorator),
                _context.Advertisement.Value.SourceUrl
            );
            return Option<ScrapedAdvertisement>.None();
        }

        await using IElementHandle priceElement = priceContainer.Value;
        Option<IElementHandle> priceNodeElement = await priceElement.GetChildWithClassFormatter(
            priceNodeSelector
        );
        if (!priceNodeElement.HasValue)
        {
            _logger.Error(
                "{Context} no price found. Url: {Url}",
                nameof(ScrapeAdvertisementPriceDecorator),
                _context.Advertisement.Value.SourceUrl
            );
            return Option<ScrapedAdvertisement>.None();
        }

        await using IElementHandle priceNode = priceNodeElement.Value;
        Option<string> price = await priceNode.GetElementText();
        if (!price.HasValue)
        {
            _logger.Error(
                "{Context} no price found. Url: {Url}",
                nameof(ScrapeAdvertisementPriceDecorator),
                _context.Advertisement.Value.SourceUrl
            );
            return Option<ScrapedAdvertisement>.None();
        }
        string priceValue = price.Value;
        string onlyDigits = new([.. priceValue.Where(char.IsDigit).Select(c => c)]);
        long priceNumber = long.Parse(onlyDigits);

        _context.Advertisement.Value.Price = priceNumber;
        Option<IElementHandle> extraInformation = await priceElement.GetChildWithClassFormatter(
            priceExtraSelector
        );
        if (!extraInformation.HasValue)
            return await _handler.Handle(command);

        await using IElementHandle extraInformationElement = extraInformation.Value;
        Option<string> extraInformationText = await extraInformationElement.GetElementText();
        if (!extraInformationText.HasValue)
            return await _handler.Handle(command);

        _context.Advertisement.Value.PriceExtra = extraInformationText.Value;
        return await _handler.Handle(command);
    }
}
