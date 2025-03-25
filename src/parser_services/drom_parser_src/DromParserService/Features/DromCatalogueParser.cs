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
    private const int _batchSize = 5;
    private readonly SemaphoreSlim _semaphore = new(_batchSize, _batchSize);

    public async Task Handle(ScrapeAdvertisementCommand command)
    {
        _context.Browser.Value.Dispose();
        var tasks = new List<Task<Option<ScrapedAdvertisement>>>();
        foreach (var advertisement in _context.EnumerateAdvertisements())
        {
            await _semaphore.WaitAsync();
            tasks.Add(
                Task.Run(async () =>
                {
                    try
                    {
                        BrowserFactory browserFactory = new();
                        IBrowser browser = await browserFactory.CreateStealthBrowserInstance(
                            headless: true
                        );

                        var result = await ScrapeAdvertisement(advertisement, browser);
                        if (result.HasValue)
                            SinkAdvertisement(result.Value);

                        browser.Dispose();
                        return result;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        return Option<ScrapedAdvertisement>.None();
                    }
                    finally
                    {
                        _semaphore.Release();
                    }
                })
            );
        }
        await Task.WhenAll(tasks);
    }

    private async Task<Option<ScrapedAdvertisement>> ScrapeAdvertisement(
        ScrapedAdvertisement advertisement,
        IBrowser browser
    )
    {
        try
        {
            string url = advertisement.SourceUrl;
            IPage page = await browser.CreateByDomLoadStrategy(url);
            await page.ScrollBottom();
            await page.ScrollTop();
            ScrapeConcreteAdvertisementCommand command = new(page, advertisement);
            var handler = _provider.GetRequiredService<IScrapeConcreteAdvertisementHandler>();
            Option<ScrapedAdvertisement> ad = await handler.Handle(command);
            page.Dispose();
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
