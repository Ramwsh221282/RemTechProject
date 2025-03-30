using PuppeteerSharp;
using RemTechCommon.Utils.OptionPattern;
using SharedParsersLibrary.Models;

namespace DromParserService.Features.DromCatalogueScraping.ConcreteAdvertisementScraping;

public sealed record ScrapeConcreteAdvertisementCommand(
    IPage Page,
    ScrapedAdvertisement Advertisement
);

public interface IScrapeConcreteAdvertisementHandler
{
    Task<Option<ScrapedAdvertisement>> Handle(ScrapeConcreteAdvertisementCommand command);
}
