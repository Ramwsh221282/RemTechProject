using PuppeteerSharp;
using RemTechCommon.Utils.OptionPattern;
using SharedParsersLibrary.Models;
using SharedParsersLibrary.Puppeteer.Features.ElementBehavior;
using SharedParsersLibrary.Puppeteer.Features.PageBehavior;

namespace DromParserService.Features.ScrapeConcreteAdvertisement.Decorators;

public sealed class ScrapeAddressDecorator(
    IScrapeConcreteAdvertisementHandler handler,
    ScrapeConcreteAdvertisementContext context
) : IScrapeConcreteAdvertisementHandler
{
    private readonly IScrapeConcreteAdvertisementHandler _handler = handler;
    private readonly ScrapeConcreteAdvertisementContext _context = context;
    private const string _containerSelector = "css-0 epjhnwz1";
    private const string _subContainerSelector = "css-1j8ksy7 eotelyr0";
    private const string _addressNodeSelector = "css-inmjwf e162wx9x0";

    public async Task<Option<ScrapedAdvertisement>> Handle(
        ScrapeConcreteAdvertisementCommand command
    )
    {
        IPage page = _context.Page.Value;
        var containerElementRequest = await page.GetElementWithClassFormatter(_containerSelector);
        if (!containerElementRequest.HasValue)
            return Option<ScrapedAdvertisement>.None();

        await using IElementHandle containerElement = containerElementRequest.Value;
        var subContainerElementRequest = await containerElement.GetChildWithClassFormatter(
            _subContainerSelector
        );
        if (!subContainerElementRequest.HasValue)
            return Option<ScrapedAdvertisement>.None();

        await using IElementHandle subContainerElement = subContainerElementRequest.Value;
        var addressNodeElementsRequest = await subContainerElement.GetChildrenWithClassFormatter(
            _addressNodeSelector
        );
        if (!addressNodeElementsRequest.HasValue)
            return Option<ScrapedAdvertisement>.None();

        foreach (var addressNode in addressNodeElementsRequest.Value)
        {
            await using (addressNode)
            {
                Option<string> text = await addressNode.GetElementText();
                if (!text.HasValue)
                    continue;
                string textValue = text.Value;
                if (textValue.Contains("Город:"))
                {
                    string addressResult = textValue.Replace("Город:", "").Trim();
                    _context.Advertisement.Value.Address = addressResult;
                    break;
                }
            }
        }

        return await _handler.Handle(command);
    }
}
