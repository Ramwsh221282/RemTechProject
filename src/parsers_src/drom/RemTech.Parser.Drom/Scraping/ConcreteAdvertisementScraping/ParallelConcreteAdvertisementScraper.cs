using System.Collections.Concurrent;
using PuppeteerSharp;
using RemTech.Domain.AdvertisementsContext;
using RemTech.Domain.AdvertisementsContext.ValueObjects;
using RemTech.Parser.Drom.Scraping.ConcreteAdvertisementScraping.Decorators;
using RemTech.Shared.SDK.OptionPattern;
using RemTech.Shared.SDK.ResultPattern;
using SharedParsersLibrary.DatabaseSinking;
using SharedParsersLibrary.Models;
using SharedParsersLibrary.Puppeteer.BrowserCreation;
using SharedParsersLibrary.Puppeteer.Extensions;

namespace RemTech.Parser.Drom.Scraping.ConcreteAdvertisementScraping;

public sealed class ParallelConcreteAdvertisementScraper(
    string serviceName,
    int maxDegreeParallelism,
    int maxRetryCount,
    DatabaseSinkingFacade facade,
    Serilog.ILogger logger
)
{
    private readonly DatabaseSinkingFacade _facade = facade;
    private readonly ConcurrentDictionary<long, ScrapedAdvertisement> _failedResults = [];
    private readonly List<Task<Option<ScrapedAdvertisement>?>> _tasks = new(maxDegreeParallelism);
    private readonly string _serviceName = serviceName;
    private readonly int _maxRetryCount = maxRetryCount;
    private readonly Serilog.ILogger _logger = logger;
    private int _currentRetryCount;
    public int FailedResultsCount => _failedResults.Count;
    public bool IsReachedMaxCount => _currentRetryCount == _maxRetryCount;

    public async Task ExecuteScrape(IEnumerable<ScrapedAdvertisement> advertisements)
    {
        foreach (ScrapedAdvertisement advertisement in advertisements)
        {
            if (_tasks.Count == _tasks.Capacity)
            {
                _logger.Information(
                    "Concrete advertisements executing reached max parallel degree. Waiting tasks for completion."
                );
                await ProcessTasks();
                _logger.Information("Concrete advertisements executing tasks pool cleared.");
            }
            _tasks.Add(
                Task.Run(async () =>
                {
                    try
                    {
                        Option<ScrapedAdvertisement> result = await CreateScrapeAdvertisementTask(
                            advertisement
                        );
                        if (result.HasValue == false)
                        {
                            _failedResults.TryAdd(advertisement.Id, advertisement);
                            return result;
                        }

                        _failedResults.TryRemove(advertisement.Id, out ScrapedAdvertisement? _);
                        return result;
                    }
                    catch (Exception ex)
                    {
                        _logger.Fatal(
                            "{Ad} scraping resulted in exception: {Ex}",
                            advertisement.SourceUrl,
                            ex.Message
                        );
                        _failedResults.TryAdd(advertisement.Id, advertisement);
                        return null;
                    }
                })
            );
        }

        await ProcessTasks();
        _logger.Information("Concrete advertisements tasks operation completed.");
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
                await SinkAdvertisement(result.Value, _serviceName, _facade, _logger);
            }
        }
        _tasks.Clear();
    }

    private static async Task SinkAdvertisement(
        ScrapedAdvertisement advertisement,
        string serviceName,
        DatabaseSinkingFacade facade,
        Serilog.ILogger logger
    )
    {
        advertisement = advertisement with { ServiceName = serviceName };
        Result<Advertisement> domainModel = advertisement.ToAdvertisemnet();

        if (domainModel.IsFailure)
        {
            logger.Information(
                "Incorrect advertisement model: {Message}.",
                domainModel.Error.Description
            );
            return;
        }

        if (await facade.HasAdvertisement(domainModel))
        {
            logger.Information("Advertisement with id: {Id} already exists.", advertisement.Id);
            return;
        }

        await facade.InsertAdvertisement(domainModel);
        logger.Information("Advertisement with id: {Id} inserted.", advertisement.Id);

        AdvertisementCharacteristicsCollection ctxCollection = domainModel.Value.Characteristics;
        foreach (AdvertisementCharacteristic ctx in ctxCollection)
        {
            if (await facade.HasCharacteristic(ctx))
            {
                logger.Information("Charactesitic with name: {Name} already exists.", ctx.Name);
                continue;
            }
            await facade.InsertCharacteristic(ctx);
            logger.Information("Characteristic with name: {Name} added.", ctx.Name);
        }
    }

    private static async Task<Option<ScrapedAdvertisement>> CreateScrapeAdvertisementTask(
        ScrapedAdvertisement advertisement
    )
    {
        IBrowser browser = await BrowserFactory.CreateStealthBrowserInstance(headless: true);
        IPage page = await CreatePage(browser, advertisement.SourceUrl);

        ScrapeConcreteAdvertisementContext context = new();
        ScrapeConcreteAdvertisementHandler handler = new(context);
        ScrapeConcreteAdvertisementPriceExtraDecorator priceHandler = new(handler, context);
        ScrapeConcreteAdvertisementInitializerDecorator processInitializer = new(
            priceHandler,
            context
        );
        ScrapeConcreteAdvertisementCommand subCommand = new(page, advertisement);

        try
        {
            Option<ScrapedAdvertisement> result = await processInitializer.Handle(subCommand);
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
}
