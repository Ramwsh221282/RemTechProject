using AvitoParserService.Common.Cqrs;
using PuppeteerSharp;
using RemTechCommon.Utils.OptionPattern;
using SharedParsersLibrary.Models;
using SharedParsersLibrary.Puppeteer.Features.ElementBehavior;
using SharedParsersLibrary.Puppeteer.Features.PageBehavior;

namespace AvitoParserService.Features.ScrapeAdvertisement.Decorators;

public sealed class ScrapePublisher
    : ICommandHandler<ScrapeConcreteAdvertisementCommand, Option<ScrapedAdvertisement>>
{
    private const string selector = "div[data-marker='seller-info/name']";

    private readonly ICommandHandler<
        ScrapeConcreteAdvertisementCommand,
        Option<ScrapedAdvertisement>
    > _handler;

    private readonly ScrapeConcreteAdvertisementContext _context;

    public ScrapePublisher(
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
            await using IElementHandle? element = await pageExecutor.Invoke(
                new GetElementByPage(selector)
            );
            if (element == null)
                return Option<ScrapedAdvertisement>.None();

            ElementBehaviorExecutor elementExecutor = new ElementBehaviorExecutor(element);
            string? text = await elementExecutor.Invoke(new GetElementTextBehavior());
            if (string.IsNullOrWhiteSpace(text))
                return Option<ScrapedAdvertisement>.None();

            _context.SetPublisher(text);
            return await _handler.Handle(command);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception at publisher scraping");
            return Option<ScrapedAdvertisement>.None();
        }
    }
}
