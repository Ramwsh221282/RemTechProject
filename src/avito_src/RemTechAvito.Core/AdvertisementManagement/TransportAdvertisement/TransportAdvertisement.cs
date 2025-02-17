using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement.ValueObjects;
using RemTechAvito.Core.Common.ValueObjects;

namespace RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement;

public sealed class TransportAdvertisement
{
    public const string CollectionName = "Спецтехника";
    public AdvertisementID AdvertisementId { get; }
    public EntityID EntityId { get; }
    public Characteristics Characteristics { get; }
    public Address Address { get; }
    public OwnerInformation OwnerInformation { get; }
    public PhotoAttachments PhotoAttachments { get; }
    public Price Price { get; }
    public Title Title { get; }
    public Description Description { get; }

    public AdvertisementUrl Url { get; }

    public TransportAdvertisement(
        AdvertisementID advertisementId,
        Characteristics characteristics,
        Address address,
        OwnerInformation ownerInformation,
        PhotoAttachments photoAttachments,
        Price price,
        Title title,
        Description description,
        AdvertisementUrl url = null
    )
    {
        AdvertisementId = advertisementId;
        EntityId = EntityID.New();
        Characteristics = characteristics;
        Address = address;
        OwnerInformation = ownerInformation;
        PhotoAttachments = photoAttachments;
        Price = price;
        Title = title;
        Description = description;
        Url = url;
    }
}
