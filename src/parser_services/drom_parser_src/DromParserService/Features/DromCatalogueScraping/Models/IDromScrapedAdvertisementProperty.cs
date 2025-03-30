using SharedParsersLibrary.Models;

namespace DromParserService.Features.DromCatalogueScraping.Models;

public interface IDromScrapedAdvertisementProperty
{
    ScrapedAdvertisement Set(ScrapedAdvertisement advertisement);
}
