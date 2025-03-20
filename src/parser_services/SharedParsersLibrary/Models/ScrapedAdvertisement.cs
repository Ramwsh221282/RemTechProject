namespace SharedParsersLibrary.Models;

public sealed class ScrapedAdvertisement
{
    public long AdvertisementId { get; set; }
    public string[] PhotoUrls { get; set; } = [];
    public ScrapedCharacteristic[] Characteristics { get; set; } = [];
    public string Address { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public long Price { get; set; }
    public string PriceExtra { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    public string SourceUrl { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime Published { get; set; }
}
