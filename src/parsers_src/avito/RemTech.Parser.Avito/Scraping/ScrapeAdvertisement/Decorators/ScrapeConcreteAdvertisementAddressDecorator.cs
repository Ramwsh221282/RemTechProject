using PuppeteerSharp;
using RemTech.Shared.SDK.CqrsPattern.Commands;
using RemTech.Shared.SDK.OptionPattern;
using SharedParsersLibrary.Models;
using SharedParsersLibrary.Puppeteer.ElementBehavior;
using SharedParsersLibrary.Puppeteer.PageBehavior;

namespace RemTech.Parser.Avito.Scraping.ScrapeAdvertisement.Decorators;

public sealed class ScrapeConcreteAdvertisementAddressDecorator(
    ICommandHandler<ScrapeConcreteAdvertisementCommand, Option<ScrapedAdvertisement>> handler,
    ScrapeConcreteAdvertisementContext context
) : ICommandHandler<ScrapeConcreteAdvertisementCommand, Option<ScrapedAdvertisement>>
{
    private const string selector = "style-item-address__string-wt61A";
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
        Option<IElementHandle> getAddress = await page.GetElementWithClassFormatter(selector);

        if (getAddress.HasValue == false)
            return Option<ScrapedAdvertisement>.None();

        Option<string> addressText = await getAddress.Value.GetElementText();
        if (addressText.HasValue == false)
            return Option<ScrapedAdvertisement>.None();

        _context.SetAddress(addressText.Value);
        return await _handler.Handle(command, ct);
    }
}
