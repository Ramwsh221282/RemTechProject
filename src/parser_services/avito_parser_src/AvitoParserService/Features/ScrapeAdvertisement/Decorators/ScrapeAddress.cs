using AvitoParserService.Common.Cqrs;
using PuppeteerSharp;
using RemTechCommon.Utils.OptionPattern;
using SharedParsersLibrary.Models;
using SharedParsersLibrary.Puppeteer.Features.ElementBehavior;
using SharedParsersLibrary.Puppeteer.Features.PageBehavior;

namespace AvitoParserService.Features.ScrapeAdvertisement.Decorators;

public sealed class ScrapeAddress
    : ICommandHandler<ScrapeConcreteAdvertisementCommand, Option<ScrapedAdvertisement>>
{
    private const string selector = "style-item-address__string-wt61A";

    private readonly ICommandHandler<
        ScrapeConcreteAdvertisementCommand,
        Option<ScrapedAdvertisement>
    > _handler;

    private readonly ScrapeConcreteAdvertisementContext _context;

    public ScrapeAddress(
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
            PageBehaviorExecutor executor = new PageBehaviorExecutor(page);
            IPageBehavior<IElementHandle?> getTitle = new GetElementWithSelectorFormat(selector);
            await using IElementHandle? title = await executor.Invoke(getTitle);
            if (title == null)
                return Option<ScrapedAdvertisement>.None();
            ElementBehaviorExecutor elementExecutor = new ElementBehaviorExecutor(title);
            string? text = await elementExecutor.Invoke(new GetElementTextBehavior());
            if (string.IsNullOrEmpty(text))
                return Option<ScrapedAdvertisement>.None();
            _context.SetAddress(text);
            return await _handler.Handle(command);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception at address scraping");
            return Option<ScrapedAdvertisement>.None();
        }
    }
}
