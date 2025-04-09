using RemTech.Parser.Avito.Scraping.ScrapeAdvertisement.Decorators;
using RemTech.Shared.SDK.CqrsPattern.Commands;
using RemTech.Shared.SDK.OptionPattern;
using Serilog;
using SharedParsersLibrary.Models;

namespace RemTech.Parser.Avito.Scraping.ScrapeAdvertisement;

public sealed class ScrapeConcreteAdvertisementFactory(Serilog.ILogger logger)
{
    private readonly Serilog.ILogger _logger = logger;

    public ICommandHandler<
        ScrapeConcreteAdvertisementCommand,
        Option<ScrapedAdvertisement>
    > Create()
    {
        ScrapeConcreteAdvertisementContext context = new();

        ScrapeConcreteAdvertisementCommandHandler coreHandler = new(context);
        ScrapeConcreteAdvertisementAddressDecorator addressHandler = new(coreHandler, context);
        ScrapeConcreteAdvertisementCharacteristicsDecorator ctxHandler = new(
            addressHandler,
            context
        );
        ScrapeConcreteAdvertisementDateDecorator dateHandler = new(ctxHandler, context);
        ScrapeConcreteAdvertisementDescriptionDecorator descHandler = new(dateHandler, context);
        ScrapeConcreteAdvertisementPriceDecorator priceHandler = new(descHandler, context);
        ScrapeConcreteAdvertisementPublisherDecorator publisherHandler = new(priceHandler, context);
        ScrapeConcreteAdvertisementTitleDecorator titleHandler = new(publisherHandler, context);
        ScrapeConcreteAdvertisementUrlDecorator urlHandler = new(titleHandler, context, _logger);
        ScrapeConcreteAdvertisementExceptionSupressorDecorator exceptionHandler = new(
            urlHandler,
            _logger
        );
        ScrapeConcreteAdvertisementPageFinalizerDecorator pageFinalizer = new(exceptionHandler);
        return pageFinalizer;
    }
}
