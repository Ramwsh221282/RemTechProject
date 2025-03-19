using DromParserService.Features.Decorators;
using DromParserService.Features.ScrapeConcreteAdvertisement;
using Serilog;
using SharedParsersLibrary.Attributes;
using SharedParsersLibrary.Contracts;
using SharedParsersLibrary.Sinking;
using ILogger = Serilog.ILogger;

namespace DromParserService.Features;

[ParserDependencyInjection]
public static class DromCatalogueParserDependencyInjection
{
    [ParserDependencyInjectionMethod]
    public static void RegisterCatalogueScraper(this IServiceCollection services)
    {
        services.AddTransient<IScrapeAdvertisementsHandler>(p =>
        {
            ILogger logger = p.GetRequiredService<ILogger>();
            SinkerSenderFactory factory = p.GetRequiredService<SinkerSenderFactory>();
            DromCatalogueScrapingContext context = new DromCatalogueScrapingContext();
            DromCatalogueParser h1 = new DromCatalogueParser(factory, context, p);
            DromScrapeCataloguePages h2 = new DromScrapeCataloguePages(h1, context, logger);
            DromInitializeCatalogueParser h3 = new DromInitializeCatalogueParser(h2, context);
            return h3;
        });
    }
}
