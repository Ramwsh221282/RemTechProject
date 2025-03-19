using AvitoParserService.Common.Cqrs;
using PuppeteerSharp;
using RemTechCommon.Utils.OptionPattern;
using SharedParsersLibrary.Models;
using SharedParsersLibrary.Puppeteer.Features.ElementBehavior;
using SharedParsersLibrary.Puppeteer.Features.PageBehavior;

namespace AvitoParserService.Features.ScrapeAdvertisement.Decorators;

public sealed class ScrapeTitle
    : ICommandHandler<ScrapeConcreteAdvertisementCommand, Option<ScrapedAdvertisement>>
{
    private const string selector = "h1[data-marker='item-view/title-info']";
    private readonly ScrapeConcreteAdvertisementContext _context;

    private readonly ICommandHandler<
        ScrapeConcreteAdvertisementCommand,
        Option<ScrapedAdvertisement>
    > _handler;

    public ScrapeTitle(
        ICommandHandler<ScrapeConcreteAdvertisementCommand, Option<ScrapedAdvertisement>> handler,
        ScrapeConcreteAdvertisementContext context
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
            if (string.IsNullOrWhiteSpace(text))
                return Option<ScrapedAdvertisement>.None();

            _context.SetTitle(text);
            return await _handler.Handle(command);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception at title scraping");
            return Option<ScrapedAdvertisement>.None();
        }
    }
}
