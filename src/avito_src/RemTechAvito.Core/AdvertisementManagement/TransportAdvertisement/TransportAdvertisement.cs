using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement.ValueObjects;
using RemTechAvito.Core.Common.ValueObjects;

namespace RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement;

public sealed class TransportAdvertisement
{
    public const string CollectionName = "Спецтехника";
    public AdvertisementID AdvertisementId { get; }
    public EntityID EntityId { get; }
    public Characteristic Characteristic { get; }
    public Address Address { get; }
    public OwnerInformation OwnerInformation { get; }
    public PhotoAttachments PhotoAttachments { get; }
    public Price Price { get; }
    public Title Title { get; }
    public Description Description { get; }

    public TransportAdvertisement(
        AdvertisementID advertisementId,
        Characteristic characteristic,
        Address address,
        OwnerInformation ownerInformation,
        PhotoAttachments photoAttachments,
        Price price,
        Title title,
        Description description
    )
    {
        AdvertisementId = advertisementId;
        EntityId = EntityID.New();
        Characteristic = characteristic;
        Address = address;
        OwnerInformation = ownerInformation;
        PhotoAttachments = photoAttachments;
        Price = price;
        Title = title;
        Description = description;
    }
}
