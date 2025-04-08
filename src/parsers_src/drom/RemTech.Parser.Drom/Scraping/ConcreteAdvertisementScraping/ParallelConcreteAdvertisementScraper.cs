using System.Collections.Concurrent;
using PuppeteerSharp;
using RemTech.Shared.SDK.OptionPattern;
using SharedParsersLibrary.Models;
using SharedParsersLibrary.Puppeteer.BrowserCreation;
using SharedParsersLibrary.Puppeteer.Extensions;

namespace RemTech.Parser.Drom.Scraping.ConcreteAdvertisementScraping;

public sealed class ParallelConcreteAdvertisementScraper(
    string serviceName,
    int maxDegreeParallelism,
    int maxRetryCount,
    IServiceProvider provider
)
{
    private readonly IServiceProvider _provider = provider;
    private readonly ConcurrentDictionary<long, ScrapedAdvertisement> _failedResults = [];
    private readonly List<Task<Option<ScrapedAdvertisement>?>> _tasks = new(maxDegreeParallelism);
    private readonly string _serviceName = serviceName;
    private int _currentRetryCount;
    public int FailedResultsCount => _failedResults.Count;
    public int MaxRetryCount { get; } = maxRetryCount;
    public bool IsReachedMaxCount => _currentRetryCount == MaxRetryCount;

    public async Task ExecuteScrape(IEnumerable<ScrapedAdvertisement> advertisements)
    {
        foreach (ScrapedAdvertisement advertisement in advertisements)
        {
            if (_tasks.Count == _tasks.Capacity)
                await ProcessTasks();
            _tasks.Add(
                Task.Run(async () =>
                {
                    try
                    {
                        Option<ScrapedAdvertisement> result = await CreateScrapeAdvertisementTask(
                            advertisement,
                            _provider
                        );

                        _failedResults.TryRemove(advertisement.Id, out ScrapedAdvertisement _);
                        return result;
                    }
                    catch
                    {
                        _failedResults.TryAdd(advertisement.Id, advertisement);
                        return null;
                    }
                })
            );
        }

        await ProcessTasks();
    }

    public async Task ExecuteScrape()
    {
        await ExecuteScrape(_failedResults.Values);
        _currentRetryCount++;
    }

    private async Task ProcessTasks()
    {
        await foreach (Task<Option<ScrapedAdvertisement>?> task in Task.WhenEach(_tasks))
        {
            Option<ScrapedAdvertisement>? result = await task;
            if (result == null)
                continue;

            if (result.HasValue)
            {
                SinkAdvertisement(result.Value, _serviceName);
            }
        }
        _tasks.Clear();
    }

    private static void SinkAdvertisement(ScrapedAdvertisement advertisement, string serviceName)
    {
        // TODO: LOGIC TO SINK ADVERTISEMENT IN DATABASE.
    }

    private static async Task<Option<ScrapedAdvertisement>> CreateScrapeAdvertisementTask(
        ScrapedAdvertisement advertisement,
        IServiceProvider provider
    )
    {
        IBrowser browser = await BrowserFactory.CreateStealthBrowserInstance();
        IPage page = await CreatePage(browser, advertisement.SourceUrl);
        IScrapeConcreteAdvertisementHandler handler = GetHandler(provider);
        ScrapeConcreteAdvertisementCommand subCommand = new(page, advertisement);

        try
        {
            Option<ScrapedAdvertisement> result = await handler.Handle(subCommand);
            browser.Dispose();

            return result;
        }
        finally
        {
            browser.Dispose();
        }
    }

    private static async Task<IPage> CreatePage(IBrowser browser, string url) =>
        await browser.CreateByDomLoadNoImages(url);

    private static IScrapeConcreteAdvertisementHandler GetHandler(IServiceProvider provider) =>
        provider.GetRequiredService<IScrapeConcreteAdvertisementHandler>();
}
