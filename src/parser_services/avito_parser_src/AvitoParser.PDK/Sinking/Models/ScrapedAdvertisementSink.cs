using AvitoParser.PDK.Models;
using AvitoParser.PDK.Models.ValueObjects;

namespace AvitoParser.PDK.Sinking.Models;

public sealed record ScrapedAdvertisementSink
{
    public ulong AdvertisementId { get; } = 0;
    public DateTime AdvertisementDate { get; } = DateTime.MinValue;
    public string Title { get; } = string.Empty;
    public string Description { get; } = string.Empty;
    public string Address { get; } = string.Empty;
    public string SourceUrl { get; } = String.Empty;
    public ulong PriceValue { get; } = 0;
    public string PriceDescription { get; } = string.Empty;
    public string Publisher { get; } = string.Empty;
    public string[] PhotoUrls { get; } = [];
    public ScrapedCharacteristicSink[] Characteristics { get; } = [];

    public ScrapedAdvertisementSink(ScrapedAdvertisement advertisement)
    {
        AdvertisementId = advertisement.Id.Id;
        AdvertisementDate = AdvertisementDate.Date.Date;
        Title = advertisement.Title.Title;
        Description = advertisement.Description.Description;
        Address = advertisement.Address.Address;
        SourceUrl = advertisement.SourceUrl.SourceUrl;
        PriceValue = advertisement.Price.Price;
        PriceDescription = advertisement.Price.Description;
        Publisher = advertisement.Publisher.Name;
        PhotoUrls = advertisement.Photos.Select(p => p.SourceUrl).ToArray();
        Characteristics = advertisement
            .Characteristics.Select(c => new ScrapedCharacteristicSink(c))
            .ToArray();
    }

    public static ScrapedAdvertisementSink[] FromEnumerable(
        IEnumerable<ScrapedAdvertisement> advertisements
    ) => advertisements.Select(s => new ScrapedAdvertisementSink(s)).ToArray();
}

public sealed record ScrapedCharacteristicSink
{
    public string Name { get; } = string.Empty;
    public string Value { get; } = string.Empty;

    public ScrapedCharacteristicSink(ScrapedCharacteristic ctx)
    {
        Name = ctx.Name;
        Value = ctx.Value;
    }
}

public sealed record ScrapedFromSink
{
    public string Name { get; } = string.Empty;

    public ScrapedFromSink(string name) => Name = name;
}
