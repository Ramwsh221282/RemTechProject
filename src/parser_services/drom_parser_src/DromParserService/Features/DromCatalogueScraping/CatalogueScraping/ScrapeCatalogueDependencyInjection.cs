using DromParserService.Features.DromCatalogueScraping.CatalogueScraping.Decorators;
using SharedParsersLibrary.Attributes;
using SharedParsersLibrary.Contracts;
using SharedParsersLibrary.Sinking;

namespace DromParserService.Features.DromCatalogueScraping.CatalogueScraping;

[ParserDependencyInjection]
public static class ScrapeCatalogueDependencyInjection
{
    [ParserDependencyInjectionMethod]
    public static void Inject(this IServiceCollection services)
    {
        services.AddTransient<IScrapeAdvertisementsHandler>(p =>
        {
            SinkerSenderFactory factory = p.GetRequiredService<SinkerSenderFactory>();
            Serilog.ILogger logger = p.GetRequiredService<Serilog.ILogger>();
            DromCatalogueScrapingContext context = new();
            DromScrapeCatalogueHandler h1 = new(p, factory, context);
            DromScrapeCataloguePagesDecorator h2 = new(h1, context, logger);
            DromScrapeCatalogueInitializeDecorator h3 = new(h2, context);
            return h3;
        });
    }
}
