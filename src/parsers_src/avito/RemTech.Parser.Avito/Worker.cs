using RemTech.Parser.Avito.Scraping.ScrapeAdvertisement;
using RemTech.Parser.Avito.Scraping.ScrapeCatalogue;
using SharedParsersLibrary.Contracts;
using SharedParsersLibrary.DatabaseSinking;
using SharedParsersLibrary.Models;

namespace RemTech.Parser.Avito;

public sealed class Worker(
    string serviceName,
    Serilog.ILogger logger,
    DatabaseSinkingFacade sinkingFacade,
    ParserManagementFacade parserFacade
) : BackgroundService
{
    private readonly string _serviceName = serviceName;
    private readonly Serilog.ILogger _logger = logger;
    private readonly DatabaseSinkingFacade _sinkingFacade = sinkingFacade;
    private readonly ParserManagementFacade _parserFacade = parserFacade;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            ScrapeConcreteAdvertisementFactory concreteAdvertisementFactory = new(_logger);

            IScrapeAdvertisementHandlerFactory handlerFactory = new ScrapeCatalogueFactory(
                _serviceName,
                _logger,
                _sinkingFacade,
                concreteAdvertisementFactory
            );

            ParserProcess process = new(_logger, _serviceName, _parserFacade, handlerFactory);

            try
            {
                await process.Invoke(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.Fatal("{Parser} exception thrown: {Ex}", _serviceName, ex.Message);
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
