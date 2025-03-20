using PuppeteerSharp;
using RemTechCommon.Utils.OptionPattern;
using SharedParsersLibrary.Models;

namespace DromParserService.Features.ScrapeConcreteAdvertisement;

public sealed class ScrapeConcreteAdvertisementContext
{
    public Option<IPage> Page { get; set; } = Option<IPage>.None();
    public Option<ScrapedAdvertisement> Advertisement { get; set; } =
        Option<ScrapedAdvertisement>.None();
}
