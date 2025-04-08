using RemTech.Parser.Drom.Scraping.CatalogueScraping;
using RemTech.Parser.Drom.Scraping.ConcreteAdvertisementScraping;
using SharedParsersLibrary.Contracts;
using SharedParsersLibrary.DatabaseSinking;
using SharedParsersLibrary.Models;

namespace RemTech.Parser.Drom;

public sealed class Worker(
    Serilog.ILogger logger,
    DatabaseSinkingFacade sinkingFacade,
    ParserManagementFacade parserFacade,
    string parserName
) : BackgroundService
{
    private readonly Serilog.ILogger _logger = logger;
    private readonly DatabaseSinkingFacade _sinkingFacade = sinkingFacade;
    private readonly ParserManagementFacade _parserFacade = parserFacade;
    private readonly string _parserName = parserName;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            ConcreteAdvertisementScraperFactory concreteAdvertisementScraperFactory = new(
                _sinkingFacade,
                _parserName,
                5,
                3,
                _logger
            );

            IScrapeAdvertisementHandlerFactory factory = new DromCatalogueScraperFactory(
                concreteAdvertisementScraperFactory,
                _logger
            );

            ParserProcess process = new(_logger, _parserName, _parserFacade, factory);

            try
            {
                await process.Invoke(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.Fatal("{Parser} exception thrown: {Ex}", _parserName, ex.Message);
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
