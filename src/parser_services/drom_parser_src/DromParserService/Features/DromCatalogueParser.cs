using DromParserService.Features.ScrapeConcreteAdvertisement;
using PuppeteerSharp;
using RemTechCommon.Utils.OptionPattern;
using SharedParsersLibrary.Contracts;
using SharedParsersLibrary.Models;
using SharedParsersLibrary.Puppeteer.Features.BrowserCreation;
using SharedParsersLibrary.Puppeteer.Features.PageBehavior;
using SharedParsersLibrary.Sinking;

namespace DromParserService.Features;

public sealed record ScrapingResult(IPage Page, Option<ScrapedAdvertisement> Result);

public sealed class DromCatalogueParser(
    SinkerSenderFactory factory,
    DromCatalogueScrapingContext context,
    IServiceProvider provider
) : IScrapeAdvertisementsHandler
{
    private const int BatchSize = 5;
    private readonly DromCatalogueScrapingContext _context = context;
    private readonly IServiceProvider _provider = provider;
    private readonly SinkerSenderFactory _factory = factory;

    public async Task Handle(ScrapeAdvertisementCommand command)
    {
        IBrowser browser = _context.Browser.Value;
        List<Task<ScrapingResult>> results = [];

        foreach (var advertisement in _context.EnumerateAdvertisements())
        {
            IPage page = await browser.CreateByDomLoadStrategy(advertisement.SourceUrl);
            await page.ScrollBottom();
            await page.ScrollTop();

            while (results.Count == BatchSize)
            {
                await ResolveFinishedTasks(results);
            }

            results.Add(CreateScrapeTask(advertisement, page));
        }

        while (results.Count != 0)
        {
            await ResolveFinishedTasks(results);
        }

        browser.Dispose();
        _context.Browser.Value.Dispose();
    }

    private async Task<ScrapingResult> CreateScrapeTask(
        ScrapedAdvertisement advertisement,
        IPage page
    )
    {
        Option<ScrapedAdvertisement> result = await ScrapeAdvertisement(advertisement, page);
        return new ScrapingResult(page, result);
    }

    private async Task ResolveFinishedTasks(List<Task<ScrapingResult>> collection)
    {
        Task<ScrapingResult> task = await Task.WhenAny(collection);
        collection.Remove(task);
        ScrapingResult result = await task;
        result.Page.Dispose();
        if (result.Result.HasValue)
            SinkAdvertisement(result.Result.Value);
        await Task.CompletedTask;
    }

    private async Task<Option<ScrapedAdvertisement>> ScrapeAdvertisement(
        ScrapedAdvertisement advertisement,
        IPage page
    )
    {
        try
        {
            ScrapeConcreteAdvertisementCommand command = new(page, advertisement);
            var handler = _provider.GetRequiredService<IScrapeConcreteAdvertisementHandler>();
            Option<ScrapedAdvertisement> ad = await handler.Handle(command);
            return ad;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return Option<ScrapedAdvertisement>.None();
        }
    }

    private async Task SinkAdvertisement(ScrapedAdvertisement advertisement)
    {
        var sinker = _factory.CreateSinker();
        await sinker.Sink(advertisement, "DROM");
        sinker.Dispose();
    }
}
