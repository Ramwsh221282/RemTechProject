namespace RemTech.MongoDb.Service.Features.AdvertisementsSinking;

public sealed class SinkingAdvertisement
{
    public long AdvertisementId { get; set; }

    public string[] PhotoUrls { get; set; } = [];
    public SinkingCharacteristics[] Characteristics { get; set; } = [];
    public string Address { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public long Price { get; set; }
    public string Publisher { get; set; } = string.Empty;
    public string SourceUrl { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;

    public string FromParser { get; set; } = string.Empty;
    public string PriceExtra { get; set; } = string.Empty;

    public DateTime Published { get; set; }
}

public sealed class SinkingCharacteristics
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
