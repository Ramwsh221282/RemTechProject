using PuppeteerSharp;
using RemTech.Domain.AdvertisementsContext;
using RemTech.Domain.AdvertisementsContext.ValueObjects;
using RemTech.Parser.Avito.Scraping.Authorization;
using RemTech.Parser.Avito.Scraping.ScrapeAdvertisement;
using RemTech.Shared.SDK.CqrsPattern.Commands;
using RemTech.Shared.SDK.OptionPattern;
using RemTech.Shared.SDK.ResultPattern;
using SharedParsersLibrary.Contracts;
using SharedParsersLibrary.DatabaseSinking;
using SharedParsersLibrary.Models;
using SharedParsersLibrary.Puppeteer.BrowserCreation;
using SharedParsersLibrary.Puppeteer.Extensions;

namespace RemTech.Parser.Avito.Scraping.ScrapeCatalogue;

public sealed class ScrapeCatalogueHandler(
    string serviceName,
    ScrapeCatalogueContext context,
    DatabaseSinkingFacade facade,
    Serilog.ILogger logger,
    ScrapeConcreteAdvertisementFactory factory
) : IScrapeAdvertisementsHandler
{
    private readonly ScrapeCatalogueContext _context = context;
    private readonly DatabaseSinkingFacade _facade = facade;
    private readonly ScrapeConcreteAdvertisementFactory _factory = factory;
    private readonly Serilog.ILogger _logger = logger;
    private readonly string _serviceName = serviceName;

    public async Task Handle(ScrapeAdvertisementsCommand command)
    {
        _logger.Information("Processing advertisements...");

        IBrowser browser = await BrowserFactory.CreateStealthBrowserInstance(false);
        AvitoAuthorization authorization = new(browser);
        Result authorizationResult = await authorization.Authorize();
        if (authorizationResult.IsFailure)
        {
            _logger.Error("{Action} unable to authorize.", nameof(ScrapeCatalogueHandler));
            return;
        }

        foreach (ScrapedAdvertisement advertisement in _context.EnumerateAdvertisements())
        {
            ICommandHandler<
                ScrapeConcreteAdvertisementCommand,
                Option<ScrapedAdvertisement>
            > handler = _factory.Create();

            IPage page = await browser.CreateByDomLoadStrategy(advertisement.SourceUrl);

            ScrapeConcreteAdvertisementCommand scrapeAdvertisementCommand = new(
                page,
                advertisement
            );

            Option<ScrapedAdvertisement> result = await handler.Handle(scrapeAdvertisementCommand);

            if (result.HasValue)
                await HandleScrapedAdvertisement(result.Value);
        }

        browser.Dispose();
        _logger.Information("Advertisements process finished.");
    }

    private async Task HandleScrapedAdvertisement(ScrapedAdvertisement advertisement)
    {
        advertisement = advertisement with { ServiceName = _serviceName };
        Result<Advertisement> domainModel = advertisement.ToAdvertisemnet();
        if (domainModel.IsFailure)
        {
            _logger.Information(
                "{Service} advertisement {Url} is not valid: {Error}",
                _serviceName,
                advertisement.SourceUrl,
                domainModel.Error
            );

            return;
        }

        AdvertisementCharacteristicsCollection ctxCollection = domainModel.Value.Characteristics;

        if (await _facade.HasAdvertisement(domainModel))
        {
            _logger.Information(
                "{Service} advertisement {Id} already exists",
                _serviceName,
                advertisement.Id
            );

            return;
        }

        await _facade.InsertAdvertisement(domainModel);
        _logger.Information("{Service} advertisement {Id} added.", _serviceName, advertisement.Id);

        foreach (AdvertisementCharacteristic ctx in ctxCollection)
        {
            if (await _facade.HasCharacteristic(ctx))
            {
                _logger.Information(
                    "{Service} characteristic: {Name} already exists.",
                    _serviceName,
                    ctx.Name
                );
                continue;
            }

            await _facade.InsertCharacteristic(ctx);
        }
    }
}
