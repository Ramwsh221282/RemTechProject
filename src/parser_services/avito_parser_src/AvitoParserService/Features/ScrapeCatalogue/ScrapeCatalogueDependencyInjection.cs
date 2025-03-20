using AvitoParserService.Common.Cqrs;
using AvitoParserService.Features.ScrapeAdvertisement;
using AvitoParserService.Features.ScrapeCatalogue.Decorators;
using RemTechCommon.Utils.OptionPattern;
using SharedParsersLibrary.Attributes;
using SharedParsersLibrary.Contracts;
using SharedParsersLibrary.Models;
using SharedParsersLibrary.Puppeteer.Features.BrowserCreation;
using SharedParsersLibrary.Sinking;
using ILogger = Serilog.ILogger;

namespace AvitoParserService.Features.ScrapeCatalogue;

[ParserDependencyInjection]
public static class ScrapeCatalogueDependencyInjection
{
    [ParserDependencyInjectionMethod]
    public static void RegisterCatalogueScraping(this IServiceCollection services)
    {
        services.AddTransient<IScrapeAdvertisementsHandler>(p =>
        {
            ILogger logger = p.GetRequiredService<ILogger>();
            SinkerSenderFactory factory = p.GetRequiredService<SinkerSenderFactory>();
            var subHandler = p.GetRequiredService<
                ICommandHandler<ScrapeConcreteAdvertisementCommand, Option<ScrapedAdvertisement>>
            >();
            BrowserFactory browserFactory = new();
            ScrapeCatalogueContext context = new();
            ScrapeCatalogueHandler h1 = new(factory, context, subHandler);
            ScrapeCatalogueLinks h2 = new ScrapeCatalogueLinks(h1, context, logger);
            ScrapeCatalogueBuildPagination h3 = new(h2, context, logger);
            ScrapeCatalogueInstallBrowser h4 = new(context, h3, browserFactory, logger);
            return h4;
        });
    }
}
