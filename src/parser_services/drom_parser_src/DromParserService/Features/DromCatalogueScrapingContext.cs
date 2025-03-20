using PuppeteerSharp;
using RemTechCommon.Utils.OptionPattern;
using SharedParsersLibrary.Models;

namespace DromParserService.Features;

public sealed class DromCatalogueScrapingContext
{
    private readonly LinkedList<ScrapedAdvertisement> _advertisements = [];

    public int AdvertisementsCount { get; private set; }

    public Option<IBrowser> Browser { get; set; } = Option<IBrowser>.None();

    public void AddAdvertisement(ScrapedAdvertisement advertisement) =>
        _advertisements.AddLast(advertisement);

    public IEnumerable<ScrapedAdvertisement> EnumerateAdvertisements()
    {
        while (_advertisements.First != null)
        {
            ScrapedAdvertisement advertisement = _advertisements.First.Value;
            _advertisements.RemoveFirst();
            yield return advertisement;
        }
    }
}
