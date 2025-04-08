using RemTech.Shared.SDK.OptionPattern;
using SharedParsersLibrary.Models;

namespace RemTech.Parser.Drom.Scraping.ConcreteAdvertisementScraping;

public interface IScrapeConcreteAdvertisementHandler
{
    Task<Option<ScrapedAdvertisement>> Handle(ScrapeConcreteAdvertisementCommand command);
}
