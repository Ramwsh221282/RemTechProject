using AvitoParser.Main.Models.AdvertisementsManagement.ValueObjects;

namespace AvitoParser.Main.Models.AdvertisementsManagement;

public sealed class Advertisement
{
    public RecordId RecordId { get; private init; }
    public AdvertisementId AdvertisementId { get; private init; }
    public Address Address { get; private init; }
    public AdvertisementDate AdvertisementDate { get; private init; }
    public DateCreated DateCreated { get; private init; }
    public Publisher Publisher { get; private init; }
    public Title Title { get; private init; }
    public Price Price { get; private init; }
    public Description Description { get; private init; }
    public SourceUrl SourceUrl { get; private init; }
    public CharacteristicsList Characteristics { get; private init; }
    public PhotoList Photos { get; private init; }

    public Advertisement(
        AdvertisementId advertisementId,
        Address address,
        AdvertisementDate advertisementDate,
        DateCreated dateCreated,
        Publisher publisher,
        Title title,
        Price price,
        Description description,
        SourceUrl sourceUrl,
        CharacteristicsList characteristics,
        PhotoList photos
    )
    {
        RecordId = RecordId.CreateNew();
        AdvertisementId = advertisementId;
        Address = address;
        AdvertisementDate = advertisementDate;
        DateCreated = dateCreated;
        Publisher = publisher;
        Title = title;
        Price = price;
        Description = description;
        SourceUrl = sourceUrl;
        Characteristics = characteristics;
        Photos = photos;
    }
}
