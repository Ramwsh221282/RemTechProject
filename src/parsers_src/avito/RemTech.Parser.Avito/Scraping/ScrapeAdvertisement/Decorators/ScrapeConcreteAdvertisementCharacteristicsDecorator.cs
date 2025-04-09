using PuppeteerSharp;
using RemTech.Shared.SDK.CqrsPattern.Commands;
using RemTech.Shared.SDK.OptionPattern;
using SharedParsersLibrary.Models;
using SharedParsersLibrary.Puppeteer.ElementBehavior;
using SharedParsersLibrary.Puppeteer.PageBehavior;

namespace RemTech.Parser.Avito.Scraping.ScrapeAdvertisement.Decorators;

public sealed class ScrapeConcreteAdvertisementCharacteristicsDecorator(
    ICommandHandler<ScrapeConcreteAdvertisementCommand, Option<ScrapedAdvertisement>> handler,
    ScrapeConcreteAdvertisementContext context
) : ICommandHandler<ScrapeConcreteAdvertisementCommand, Option<ScrapedAdvertisement>>
{
    private const string container = "params-paramsList-_awNW";
    private const string item = "params-paramsList__item-_2Y2O";

    private readonly ICommandHandler<
        ScrapeConcreteAdvertisementCommand,
        Option<ScrapedAdvertisement>
    > _handler = handler;

    private readonly ScrapeConcreteAdvertisementContext _context = context;

    public async Task<Option<ScrapedAdvertisement>> Handle(
        ScrapeConcreteAdvertisementCommand command,
        CancellationToken ct = default
    )
    {
        IPage page = command.Page;

        Option<IElementHandle> parent = await page.GetElementWithClassFormatter(container);
        if (parent.HasValue == false)
            return Option<ScrapedAdvertisement>.None();

        Option<IElementHandle[]> children = await parent.Value.GetChildrenWithClassFormatter(item);
        if (children.HasValue == false)
            return Option<ScrapedAdvertisement>.None();

        List<ScrapedCharacteristic> characteristics = [];
        foreach (IElementHandle child in children.Value)
        {
            Option<string> text = await child.GetElementText();
            if (text.HasValue == false)
                continue;

            characteristics.Add(CreateTextFromCharacteristicPair(text.Value));
        }

        _context.SetCharacteristics(characteristics.ToArray());
        return await _handler.Handle(command, ct);
    }

    private static ScrapedCharacteristic CreateTextFromCharacteristicPair(string pair)
    {
        string[] splitted = pair.Split(':');
        string name = splitted[0];
        string value = splitted[1];
        value = value.Replace("?", "").Replace("*", "");
        return new ScrapedCharacteristic(name, value);
    }
}
