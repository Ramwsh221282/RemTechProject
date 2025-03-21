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
        List<Task<Option<ScrapedAdvertisement>>> tasks = [];
        int batchSize = 5;
        foreach (var advertisement in _context.EnumerateAdvertisements())
        {
            if (tasks.Count == batchSize)
            {
                while (tasks.Count > 0)
                    await ResolveTasks(results, tasks);
            }
            tasks.Add(ScrapeAdvertisement(advertisement, browser));
        }
        while (tasks.Count > 0)
            await ResolveTasks(results, tasks);

        await browser.DisposeAsync();
    }

    private async Task ResolveTasks(
        List<ScrapedAdvertisement> results,
        List<Task<Option<ScrapedAdvertisement>>> tasks
    )
    {
        while (tasks.Count != 0)
        {
            var task = await Task.WhenAny(tasks);
            tasks.Remove(task);
            var result = await task;
            if (result.HasValue)
                SinkAdvertisement(result.Value);
        }
    }

    private async Task SinkAdvertisement(ScrapedAdvertisement advertisement)
    {
        using var sinker = _factory.CreateSinker();
        await sinker.Sink(advertisement, "DROM");
    }

    private async Task<Option<ScrapedAdvertisement>> ScrapeAdvertisement(
        ScrapedAdvertisement advertisement,
        IBrowser browser
    )
    {
        string url = advertisement.SourceUrl;
        await using IPage page = await browser.CreateByDomLoadStrategy(url);
        await page.ScrollBottom();
        await page.ScrollTop();
        ScrapeConcreteAdvertisementCommand command = new(page, advertisement);
        var handler = _provider.GetRequiredService<IScrapeConcreteAdvertisementHandler>();
        Option<ScrapedAdvertisement> ad = await handler.Handle(command);
        return ad;
    }
}
