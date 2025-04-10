namespace RemTech.Domain.AdvertisementsContext.ValueObjects;

public sealed record AdvertisementDateInformation(DateTime CreatedAt, DateTime PublishedAt)
{
    public static AdvertisementDateInformation New(DateTime publishedAt) =>
        new(DateTime.UtcNow, publishedAt);
}
