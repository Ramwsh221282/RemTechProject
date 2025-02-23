using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement.ValueObjects;
using RemTechAvito.Core.Common.ValueObjects;
using RemTechCommon.Utils.Extensions;

namespace RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement;

public sealed class TransportAdvertisement
{
    public AdvertisementID AdvertisementId { get; private set; }
    public TransportAdvertisementID TransportAdvertisementId { get; private set; }
    public Characteristics Characteristics { get; private set; }
    public Address Address { get; private set; }
    public OwnerInformation OwnerInformation { get; private set; }
    public PhotoAttachments PhotoAttachments { get; private set; }
    public Price Price { get; private set; }
    public Title Title { get; private set; }
    public Description Description { get; private set; }
    public AdvertisementUrl Url { get; private set; }
    public DateOnly CreatedOn { get; private set; }

    public TransportAdvertisement(
        AdvertisementID advertisementId,
        Characteristics characteristics,
        Address address,
        OwnerInformation ownerInformation,
        PhotoAttachments photoAttachments,
        Price price,
        Title title,
        Description description,
        DateOnly createdOn,
        AdvertisementUrl url
    )
    {
        AdvertisementId = advertisementId;
        TransportAdvertisementId = new TransportAdvertisementID(GuidUtils.New());
        Characteristics = characteristics;
        Address = address;
        OwnerInformation = ownerInformation;
        PhotoAttachments = photoAttachments;
        Price = price;
        Title = title;
        Description = description;
        Url = url;
        CreatedOn = createdOn;
    }
}
