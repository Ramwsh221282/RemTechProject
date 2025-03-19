using DromParserService.Features.ScrapeConcreteAdvertisement;
using PuppeteerSharp;
using RemTechCommon.Utils.OptionPattern;
using SharedParsersLibrary.Contracts;
using SharedParsersLibrary.Models;
using SharedParsersLibrary.Puppeteer.Features.BrowserCreation;
using SharedParsersLibrary.Puppeteer.Features.PageBehavior;
using SharedParsersLibrary.Sinking;

namespace DromParserService.Features;

public sealed class DromCatalogueParser(
    SinkerSenderFactory factory,
    DromCatalogueScrapingContext context,
    IServiceProvider provider
) : IScrapeAdvertisementsHandler
{
    private readonly SinkerSenderFactory _factory = factory;
    private readonly DromCatalogueScrapingContext _context = context;
    private readonly IServiceProvider _provider = provider;

    public async Task Handle(ScrapeAdvertisementCommand command)
    {
        IBrowser browser = _context.Browser.Value;

        List<ScrapedAdvertisement> results = [];
        foreach (var advertisement in _context.EnumerateAdvertisements())
        {
            var handler = _provider.GetRequiredService<IScrapeConcreteAdvertisementHandler>();
            IPage page = await browser.CreateByDomLoadStrategy(advertisement.SourceUrl);
            await page.ScrollBottom();
            await page.ScrollTop();
            var subCommand = new ScrapeConcreteAdvertisementCommand(page, advertisement);
            Option<ScrapedAdvertisement> result = await handler.Handle(subCommand);
            if (result.HasValue)
                results.Add(result.Value);
        }

        await browser.DisposeAsync();
    }
}
