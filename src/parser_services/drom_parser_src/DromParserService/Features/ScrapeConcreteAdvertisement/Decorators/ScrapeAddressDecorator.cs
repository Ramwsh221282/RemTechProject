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
    private const string _selector = "css-inmjwf e162wx9x0";

    public async Task<Option<ScrapedAdvertisement>> Handle(
        ScrapeConcreteAdvertisementCommand command
    )
    {
        IPage page = _context.Page.Value;
        Option<IElementHandle[]> elements = await page.GetElementsArrayWithClassFormatter(
            _selector
        );
        if (!elements.HasValue)
            return await _handler.Handle(command);

        IElementHandle[] elementsArray = elements.Value;
        foreach (var element in elementsArray)
        {
            await using (element)
            {
                Option<string> text = await element.GetElementText();
                if (!text.HasValue)
                    continue;
                string textValue = text.Value;
                if (textValue.Contains("Город"))
                {
                    string[] splitted = textValue.Split(':', StringSplitOptions.TrimEntries);
                    string addressValue = splitted[^1];
                    _context.Advertisement.Value.Address = addressValue;
                    break;
                }
            }
        }

        return await _handler.Handle(command);
    }
}
