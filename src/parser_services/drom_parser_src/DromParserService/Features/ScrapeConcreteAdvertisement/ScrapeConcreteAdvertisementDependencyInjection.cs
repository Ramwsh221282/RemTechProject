using DromParserService.Features.ScrapeConcreteAdvertisement.Decorators;
using SharedParsersLibrary.Attributes;

namespace DromParserService.Features.ScrapeConcreteAdvertisement;

[ParserDependencyInjection]
public static class ScrapeConcreteAdvertisementDependencyInjection
{
    [ParserDependencyInjectionMethod]
    public static void Inject(this IServiceCollection services)
    {
        services.AddTransient<IScrapeConcreteAdvertisementHandler>(p =>
        {
            ScrapeConcreteAdvertisementContext context = new();
            Serilog.ILogger logger = p.GetRequiredService<Serilog.ILogger>();
            // ScrapeConcreteAdvertisementCommandHandler h1 = new(context);
            // ScrapeAddressDecorator h2 = new(h1, context, logger);
            // ScrapeAdvertisementDateDecorator h3 = new(h2, context, logger);
            // ScrapeAdvertisementDescriptionDecorator h4 = new(h3, context, logger);
            // ScrapeAdvertisementPriceDecorator h5 = new(h4, context, logger);
            // ScrapeCharacteristicsDecorator h6 = new(h5, context, logger);
            // ScrapePublisherDecorator h7 = new(h6, context, logger);
            // ScrapeConcreteAdvertisementInitialize h8 = new(h7, context);
            // return h8;

            ScrapeConcreteAdvertisementCommandHandler h1 = new(context);
            ScrapeAdvertisementDateDecorator h2 = new(h1, context, logger);
            ScrapeAdvertisementDescriptionDecorator h3 = new(h2, context, logger);
            ScrapeCharacteristicsDecorator h4 = new(h3, context, logger);
            ScrapeAddressDecorator h5 = new(h4, context, logger);
            ScrapePublisherDecorator h6 = new(h5, context, logger);
            ScrapeAdvertisementPriceDecorator h7 = new(h6, context, logger);
            ScrapeConcreteAdvertisementInitialize h8 = new(h7, context);
            return h8;
        });
    }
}
