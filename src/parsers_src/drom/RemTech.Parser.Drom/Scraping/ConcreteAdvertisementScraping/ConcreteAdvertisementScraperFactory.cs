using SharedParsersLibrary.DatabaseSinking;

namespace RemTech.Parser.Drom.Scraping.ConcreteAdvertisementScraping;

public sealed class ConcreteAdvertisementScraperFactory(
    DatabaseSinkingFacade facade,
    string serviceName,
    int maxDegreeParallelism,
    int maxRetryCount,
    Serilog.ILogger logger
)
{
    private readonly Serilog.ILogger _logger = logger;
    private readonly DatabaseSinkingFacade _facade = facade;
    private readonly string _serviceName = serviceName;
    private readonly int _maxDegreeParallelism = maxDegreeParallelism;
    private readonly int _maxRetryCount = maxRetryCount;

    public ParallelConcreteAdvertisementScraper Create()
    {
        return new ParallelConcreteAdvertisementScraper(
            _serviceName,
            _maxDegreeParallelism,
            _maxRetryCount,
            _facade,
            _logger
        );
    }
}
