using RemTech.Domain.AdvertisementsContext.ValueObjects;

namespace RemTech.Domain.AdvertisementsContext;

public sealed class Advertisement
{
    public AdvertisementId Id { get; private set; }
    public AdvertisementTextInformation Text { get; private set; } = null!;
    public AdvertisementPriceInformation Price { get; private set; } = null!;
    public AdvertisementScraperInformation Scraper { get; private set; } = null!;
    public AdvertisementPhotoCollection Photos { get; private set; } = null!;
    public AdvertisementCharacteristicsCollection Characteristics { get; private set; } = null!;
    public AdvertisementAddress Address { get; private set; } = null!;

    private Advertisement()
    {
        // ef core
    }

    public Advertisement(
        AdvertisementId id,
        AdvertisementTextInformation text,
        AdvertisementPriceInformation price,
        AdvertisementScraperInformation scraper,
        AdvertisementPhotoCollection photos,
        AdvertisementCharacteristicsCollection characteristics,
        AdvertisementAddress address
    )
    {
        Id = id;
        Text = text;
        Price = price;
        Scraper = scraper;
        Photos = photos;
        Characteristics = characteristics;
        Address = address;
    }
}
