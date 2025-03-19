using RemTechCommon.Utils.OptionPattern;
using SharedParsersLibrary.Models;

namespace DromParserService.Features.ScrapeConcreteAdvertisement;

public interface IScrapeConcreteAdvertisementHandler
{
    Task<Option<ScrapedAdvertisement>> Handle(ScrapeConcreteAdvertisementCommand command);
}
