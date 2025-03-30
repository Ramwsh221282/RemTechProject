using DromParserService.Features.DromCatalogueScraping.ConcreteAdvertisementScraping.Decorators;
using SharedParsersLibrary.Attributes;

namespace DromParserService.Features.DromCatalogueScraping.ConcreteAdvertisementScraping;

[ParserDependencyInjection]
public static class ScrapeConcreteAdvertisementDependencyInjection
{
    [ParserDependencyInjectionMethod]
    public static void Inject(this IServiceCollection services)
    {
        services.AddTransient<IScrapeConcreteAdvertisementHandler>(_ =>
        {
            ScrapeConcreteAdvertisementContext context = new();
            ScrapeConcreteAdvertisementHandler h1 = new(context);
            ScrapeConcreteAdvertisementPriceExtraDecorator h2 = new(h1, context);
            ScrapeConcreteAdvertisementInitializerDecorator h3 = new(h2, context);
            return h3;
        });
    }
}
