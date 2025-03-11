using AvitoParser.PDK.Models.ValueObjects;

namespace AvitoParser.PDK.Models;

public sealed record ScrapedAdvertisement
{
    public List<ScrapedCharacteristic> Characteristics { get; init; } = [];
    public List<ScrapedPhotoUrl> Photos { get; init; } = [];
    public ScrapedAddress Address { get; init; } = ScrapedAddress.Default;
    public ScrapedAdvertisementDate Date { get; init; } = ScrapedAdvertisementDate.Default;
    public ScrapedAdvertisementId Id { get; init; } = ScrapedAdvertisementId.Default;
    public ScrapedDescription Description { get; init; } = ScrapedDescription.Default;
    public ScrapedPrice Price { get; init; } = ScrapedPrice.Default;
    public ScrapedPublisher Publisher { get; init; } = ScrapedPublisher.Default;
    public ScrapedSourceUrl SourceUrl { get; init; } = ScrapedSourceUrl.Default;
    public ScrapedTitle Title { get; init; } = ScrapedTitle.Default;
}
