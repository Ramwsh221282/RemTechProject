namespace SharedParsersLibrary.Models;

public sealed record ScrapedAdvertisement(
    long Id,
    string Address,
    string Description,
    long Price,
    string PriceExtra,
    string Publisher,
    string ServiceName,
    string SourceUrl,
    string Title,
    DateTime Published,
    ScrapedCharacteristic[] Characteristics,
    string[] PhotoUrls
)
{
    public static ScrapedAdvertisement Default() =>
        new(
            0,
            string.Empty,
            string.Empty,
            0,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            default,
            [],
            []
        );
}
