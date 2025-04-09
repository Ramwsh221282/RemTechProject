using RemTech.Parser.Avito.Scraping.ScrapeAdvertisement;
using RemTech.Parser.Avito.Scraping.ScrapeCatalogue.Decorators;
using SharedParsersLibrary.Contracts;
using SharedParsersLibrary.DatabaseSinking;

namespace RemTech.Parser.Avito.Scraping.ScrapeCatalogue;

public sealed class ScrapeCatalogueFactory(
    string _serviceName,
    Serilog.ILogger logger,
    DatabaseSinkingFacade facade,
    ScrapeConcreteAdvertisementFactory concreteAdvertisementFactory
) : IScrapeAdvertisementHandlerFactory
{
    private readonly string _serviceName = _serviceName;
    private readonly Serilog.ILogger _logger = logger;
    private readonly DatabaseSinkingFacade _facade = facade;
    private readonly ScrapeConcreteAdvertisementFactory _concreteAdvertisementFactory =
        concreteAdvertisementFactory;

    public IScrapeAdvertisementsHandler Create()
    {
        ScrapeCatalogueContext context = new();

        ScrapeCatalogueHandler coreHandler = new(
            _serviceName,
            context,
            _facade,
            _logger,
            _concreteAdvertisementFactory
        );

        ScrapeCatalogueItemsDecorator itemsHandler = new(coreHandler, context, _logger);

        ScrapeCataloguePaginationDecorator paginationHandler = new(itemsHandler, context, _logger);

        ScrapeCatalogueExceptionSupressorDecorator exceptionHandler = new(
            paginationHandler,
            _logger
        );

        return exceptionHandler;
    }
}
