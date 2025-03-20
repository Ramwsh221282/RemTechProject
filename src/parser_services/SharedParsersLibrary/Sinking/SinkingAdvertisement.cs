using SharedParsersLibrary.Models;

namespace SharedParsersLibrary.Sinking;

public sealed class SinkingAdvertisement
{
    public long AdvertisementId { get; }
    public string[] PhotoUrls { get; }
    public SinkingCharacteristics[] Characteristics { get; }
    public string Address { get; }
    public string Description { get; }
    public long Price { get; }
    public string PriceExtra { get; }
    public string Publisher { get; }
    public string SourceUrl { get; }
    public string Title { get; }
    public string FromParser { get; }
    public DateTime Published { get; }

    public SinkingAdvertisement(ScrapedAdvertisement advertisement, string fromParser)
    {
        AdvertisementId = advertisement.AdvertisementId;
        PhotoUrls = advertisement.PhotoUrls;
        Characteristics = advertisement
            .Characteristics.Select(c => new SinkingCharacteristics(c))
            .ToArray();
        Address = advertisement.Address;
        Description = advertisement.Description;
        Price = advertisement.Price;
        Publisher = advertisement.Publisher;
        SourceUrl = advertisement.SourceUrl;
        Title = advertisement.Title;
        FromParser = fromParser;
        PriceExtra = advertisement.PriceExtra;
        Published = advertisement.Published;
    }
}
