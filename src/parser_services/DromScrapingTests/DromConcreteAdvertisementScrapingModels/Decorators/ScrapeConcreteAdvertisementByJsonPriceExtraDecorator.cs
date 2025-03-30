using DromParserService.Features.DromCatalogueScraping.ConcreteAdvertisementScraping;
using PuppeteerSharp;
using RemTechCommon.Utils.OptionPattern;
using SharedParsersLibrary.Models;
using SharedParsersLibrary.Puppeteer.Features.ElementBehavior;
using SharedParsersLibrary.Puppeteer.Features.PageBehavior;

namespace DromScrapingTests.DromConcreteAdvertisementScrapingModels.Decorators;

public sealed class ScrapeConcreteAdvertisementByJsonPriceExtraDecorator(
    IScrapeConcreteAdvertisementHandler handler,
    ScrapeConcreteAdvertisementByJsonContext context
) : IScrapeConcreteAdvertisementHandler
{
    private const string ContainerSelector = "css-pmcte9 e1ab30xp0";
    private const string PriceNodeSelector = "wb9m8q0";
    private const string PriceExtraSelector = "css-1oit28e ejipaoe0";
    private readonly IScrapeConcreteAdvertisementHandler _handler = handler;
    private readonly ScrapeConcreteAdvertisementByJsonContext _context = context;

    public async Task<Option<ScrapedAdvertisement>> Handle(
        ScrapeConcreteAdvertisementCommand command
    )
    {
        IPage page = command.Page;
        Option<IElementHandle> priceContainer = await page.GetElementWithClassFormatter(
            ContainerSelector
        );
        if (!priceContainer.HasValue)
            return Option<ScrapedAdvertisement>.None();

        await using IElementHandle priceElement = priceContainer.Value;
        Option<IElementHandle> priceNodeElement = await priceElement.GetChildWithClassFormatter(
            PriceNodeSelector
        );
        if (!priceNodeElement.HasValue)
            return Option<ScrapedAdvertisement>.None();

        Option<IElementHandle> extraInformation = await priceElement.GetChildWithClassFormatter(
            PriceExtraSelector
        );
        if (!extraInformation.HasValue)
        {
            _context.PriceExtra = "Без ндс";
            return await _handler.Handle(command);
        }

        await using IElementHandle extraInformationElement = extraInformation.Value;
        Option<string> extraInformationText = await extraInformationElement.GetElementText();
        if (!extraInformationText.HasValue)
        {
            _context.PriceExtra = "Без ндс";
            return await _handler.Handle(command);
        }

        _context.PriceExtra = extraInformationText.Value;
        return await _handler.Handle(command);
    }
}
