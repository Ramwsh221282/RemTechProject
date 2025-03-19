using AvitoParserService.Common.Cqrs;
using PuppeteerSharp;
using RemTechCommon.Utils.OptionPattern;
using SharedParsersLibrary.Models;
using SharedParsersLibrary.Puppeteer.Features.ElementBehavior;
using SharedParsersLibrary.Puppeteer.Features.PageBehavior;

namespace AvitoParserService.Features.ScrapeAdvertisement.Decorators;

public sealed class ScrapeCharacteristics
    : ICommandHandler<ScrapeConcreteAdvertisementCommand, Option<ScrapedAdvertisement>>
{
    private const string container = "params-paramsList-_awNW";
    private const string item = "params-paramsList__item-_2Y2O";

    private readonly ICommandHandler<
        ScrapeConcreteAdvertisementCommand,
        Option<ScrapedAdvertisement>
    > _handler;

    private readonly ScrapeConcreteAdvertisementContext _context;

    public ScrapeCharacteristics(
        ICommandHandler<ScrapeConcreteAdvertisementCommand, Option<ScrapedAdvertisement>> handler,
        ScrapeConcreteAdvertisementContext context
    )
    {
        _handler = handler;
        _context = context;
    }

    public async Task<Option<ScrapedAdvertisement>> Handle(
        ScrapeConcreteAdvertisementCommand command
    )
    {
        try
        {
            IPage page = command.Page;
            PageBehaviorExecutor pageExecutor = new PageBehaviorExecutor(page);
            await using IElementHandle? parent = await pageExecutor.Invoke(
                new GetElementWithSelectorFormat(container)
            );
            if (parent is null)
                return Option<ScrapedAdvertisement>.None();

            ElementBehaviorExecutor elementExecutor = new ElementBehaviorExecutor(parent);
            IElementHandle[] children = await elementExecutor.Invoke(
                new GetChildrenWithFormatterBehavior(item)
            );
            if (children.Length == 0)
                return Option<ScrapedAdvertisement>.None();

            List<ScrapedCharacteristic> characteristics = [];
            foreach (var child in children)
            {
                await using (child)
                {
                    elementExecutor.SwapElement(child);
                    string? text = await elementExecutor.Invoke(new GetElementTextBehavior());
                    if (string.IsNullOrWhiteSpace(text))
                        continue;
                    characteristics.Add(CreateTextFromCharacteristicPair(text));
                }
            }

            _context.SetCharacteristics(characteristics.ToArray());
            return await _handler.Handle(command);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception at characteristics scraping");
            return Option<ScrapedAdvertisement>.None();
        }
    }

    private ScrapedCharacteristic CreateTextFromCharacteristicPair(string pair)
    {
        string[] splitted = pair.Split(':');
        string name = splitted[0];
        string value = splitted[1];
        value = value.Replace("?", "").Replace("*", "");
        return new ScrapedCharacteristic { Name = name, Value = value };
    }
}
