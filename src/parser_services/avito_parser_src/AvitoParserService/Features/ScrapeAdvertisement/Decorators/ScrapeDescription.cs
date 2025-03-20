using AvitoParserService.Common.Cqrs;
using PuppeteerSharp;
using RemTechCommon.Utils.OptionPattern;
using SharedParsersLibrary.Models;
using SharedParsersLibrary.Puppeteer.Features.ElementBehavior;
using SharedParsersLibrary.Puppeteer.Features.PageBehavior;

namespace AvitoParserService.Features.ScrapeAdvertisement.Decorators;

public sealed class ScrapeDescription
    : ICommandHandler<ScrapeConcreteAdvertisementCommand, Option<ScrapedAdvertisement>>
{
    private const string selector = "div[data-marker='item-view/item-description']";

    private readonly ICommandHandler<
        ScrapeConcreteAdvertisementCommand,
        Option<ScrapedAdvertisement>
    > _handler;

    private readonly ScrapeConcreteAdvertisementContext _context;

    public ScrapeDescription(
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
            await using IElementHandle? description = await pageExecutor.Invoke(
                new GetElementByPage(selector)
            );
            if (description == null)
                return Option<ScrapedAdvertisement>.None();
            ElementBehaviorExecutor elementExecutor = new ElementBehaviorExecutor(description);
            string? text = await elementExecutor.Invoke(new GetElementTextBehavior());
            if (string.IsNullOrEmpty(text))
                return Option<ScrapedAdvertisement>.None();
            _context.SetDescription(text);
            return await _handler.Handle(command);
        }
        catch
        {
            Console.WriteLine("Exception at description scraping");
            return Option<ScrapedAdvertisement>.None();
        }
    }
}
