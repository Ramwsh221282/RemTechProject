namespace RemTech.Infrastructure.PostgreSql.AdvertisementsContext.DaoModels;

public sealed class AdvertisementDao
{
    public required long Id { get; init; }
    public required string PriceExtra { get; init; }
    public required long PriceValue { get; init; }
    public required string PublishedBy { get; init; }
    public required string ScraperName { get; init; }
    public required string SourceUrl { get; init; }
    public required string Description { get; init; }
    public required string Title { get; init; }
    public required string CharacteristicsJson { get; init; }
    public required string PhotosJson { get; init; }

    public required string Address { get; init; }

    public AdvertisementsCharacteristicsDao Characteristics =>
        AdvertisementsCharacteristicsDao.Create(this);

    public AdvertisementPhotosDao Photos => AdvertisementPhotosDao.Create(this);
}
