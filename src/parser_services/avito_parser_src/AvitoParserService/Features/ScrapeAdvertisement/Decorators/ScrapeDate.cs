using AvitoParserService.Common.Cqrs;
using AvitoParserService.Features.ScrapeAdvertisement.DateConverting;
using PuppeteerSharp;
using RemTechCommon.Utils.OptionPattern;
using SharedParsersLibrary.Models;
using SharedParsersLibrary.Puppeteer.Features.ElementBehavior;
using SharedParsersLibrary.Puppeteer.Features.PageBehavior;

namespace AvitoParserService.Features.ScrapeAdvertisement.Decorators;

public sealed class ScrapeDate
    : ICommandHandler<ScrapeConcreteAdvertisementCommand, Option<ScrapedAdvertisement>>
{
    private const string selector = "span[data-marker='item-view/item-date']";
    private readonly ScrapeConcreteAdvertisementContext _context;

    private readonly ICommandHandler<
        ScrapeConcreteAdvertisementCommand,
        Option<ScrapedAdvertisement>
    > _handler;

    public ScrapeDate(
        ScrapeConcreteAdvertisementContext context,
        ICommandHandler<ScrapeConcreteAdvertisementCommand, Option<ScrapedAdvertisement>> handler
    )
    {
        _context = context;
        _handler = handler;
    }

    public async Task<Option<ScrapedAdvertisement>> Handle(
        ScrapeConcreteAdvertisementCommand command
    )
    {
        try
        {
            IPage page = command.Page;
            PageBehaviorExecutor pageExecutor = new PageBehaviorExecutor(page);
            await using IElementHandle? element = await pageExecutor.Invoke(
                new GetElementByPage(selector)
            );
            if (element == null)
                return Option<ScrapedAdvertisement>.None();

            ElementBehaviorExecutor elementExecutor = new ElementBehaviorExecutor(element);
            string? text = await elementExecutor.Invoke(new GetElementTextBehavior());
            if (string.IsNullOrEmpty(text))
                return Option<ScrapedAdvertisement>.None();

            IDateConverter converter = new CompositeDateConverter(text);
            Option<DateTime> date = converter.Convert();
            if (!date.HasValue)
                return Option<ScrapedAdvertisement>.None();

            _context.SetDate(date.Value);
            return await _handler.Handle(command);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception at date scraping");
            return Option<ScrapedAdvertisement>.None();
        }
    }
}
