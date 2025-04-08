using PuppeteerSharp;
using RemTech.Shared.SDK.OptionPattern;
using SharedParsersLibrary.Models;

namespace RemTech.Parser.Drom.Scraping.CatalogueScraping;

public sealed class DromCatalogueScrapingContext
{
    private readonly LinkedList<ScrapedAdvertisement> _advertisements = [];
    public Option<IBrowser> Browser { get; set; } = Option<IBrowser>.None();
    private bool _isDisposed;

    public void AddAdvertisement(ScrapedAdvertisement advertisement) =>
        _advertisements.AddLast(advertisement);

    public void Dispose()
    {
        if (_isDisposed)
            return;
        if (!Browser.HasValue)
            return;
        Browser.Value.Dispose();
        _isDisposed = true;
    }

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
