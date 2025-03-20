using AvitoParserService.Common.Cqrs;
using AvitoParserService.Features.ScrapeAdvertisement.Decorators;
using RemTechCommon.Utils.OptionPattern;
using SharedParsersLibrary.Attributes;
using SharedParsersLibrary.Models;
using ILogger = Serilog.ILogger;

namespace AvitoParserService.Features.ScrapeAdvertisement;

[ParserDependencyInjection]
public static class ScrapeConcreteAdvertisementDependencyInjection
{
    [ParserDependencyInjectionMethod]
    public static void RegisterScrapeConcreteAdvertisement(this IServiceCollection services)
    {
        services.AddTransient<
            ICommandHandler<ScrapeConcreteAdvertisementCommand, Option<ScrapedAdvertisement>>
        >(p =>
        {
            ILogger logger = p.GetRequiredService<ILogger>();
            ScrapeConcreteAdvertisementContext context = new();
            ScrapeConcreteAdvertisementCommandHandler h1 =
                new ScrapeConcreteAdvertisementCommandHandler(context);
            ScrapeAddress h2 = new ScrapeAddress(h1, context);
            ScrapeCharacteristics h3 = new ScrapeCharacteristics(h2, context);
            ScrapeDate h4 = new ScrapeDate(context, h3);
            ScrapeDescription h5 = new ScrapeDescription(h4, context);
            ScrapePrice h6 = new ScrapePrice(h5, context);
            ScrapePublisher h7 = new ScrapePublisher(h6, context);
            ScrapeTitle h8 = new ScrapeTitle(h7, context);
            InitializeUrl h9 = new(context, h8, logger);
            PageFinializer h10 = new PageFinializer(h9);
            return h10;
        });
    }
}
