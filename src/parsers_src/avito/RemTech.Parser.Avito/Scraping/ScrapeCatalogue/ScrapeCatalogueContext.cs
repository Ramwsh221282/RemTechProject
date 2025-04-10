using PuppeteerSharp;
using RemTech.Shared.SDK.OptionPattern;
using SharedParsersLibrary.Models;

namespace RemTech.Parser.Avito.Scraping.ScrapeCatalogue;

public sealed class ScrapeCatalogueContext
{
    public int AdvertisementsAmount { get; private set; }

    public Option<IBrowser> Browser { get; set; } = Option<IBrowser>.None();
    public LinkedList<ScrapedAdvertisement> Advertisements { get; private set; } = [];
    public LinkedList<string> PageUrls { get; private set; } = [];

    public void AddPageUrl(string url) => PageUrls.AddLast(url);

    public void AddAdvertisement(ScrapedAdvertisement advertisement)
    {
        Advertisements.AddLast(advertisement);
        AdvertisementsAmount++;
    }

    public IEnumerable<ScrapedAdvertisement> EnumerateAdvertisements()
    {
        while (Advertisements.First != null)
        {
            ScrapedAdvertisement advertisement = Advertisements.First.Value;
            Advertisements.RemoveFirst();
            yield return advertisement;
        }
    }

    public IEnumerable<string> EnumeratePages()
    {
        while (PageUrls.First != null)
        {
            string url = PageUrls.First.Value;
            PageUrls.RemoveFirst();
            yield return url;
        }
    }
}
