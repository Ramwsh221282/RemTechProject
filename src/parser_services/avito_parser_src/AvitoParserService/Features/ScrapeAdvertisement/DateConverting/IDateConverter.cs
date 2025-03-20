using RemTechCommon.Utils.OptionPattern;

namespace AvitoParserService.Features.ScrapeAdvertisement.DateConverting;

public interface IDateConverter
{
    Option<DateTime> Convert();
}
