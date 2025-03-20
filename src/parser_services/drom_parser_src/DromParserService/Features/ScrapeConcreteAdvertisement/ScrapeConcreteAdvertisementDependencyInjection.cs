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
            ScrapeConcreteAdvertisementContext context = new ScrapeConcreteAdvertisementContext();
            ScrapeConcreteAdvertisementCommandHandler h1 = new(context);
            ScrapeAddressDecorator h2 = new(h1, context);
            ScrapeAdvertisementDateDecorator h3 = new(h2, context);
            ScrapeAdvertisementDescriptionDecorator h4 = new(h3, context);
            ScrapeAdvertisementPriceDecorator h5 = new(h4, context);
            ScrapeCharacteristicsDecorator h6 = new(h5, context);
            ScrapePublisherDecorator h7 = new(h6, context);
            ScrapeConcreteAdvertisementInitialize h8 = new(h7, context);
            return h8;
        });
    }
}
