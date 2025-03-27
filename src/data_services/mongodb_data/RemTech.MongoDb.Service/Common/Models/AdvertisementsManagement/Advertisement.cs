using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RemTech.MongoDb.Service.Common.Models.AdvertisementsManagement;

public sealed record AdvertisementCharacteristic(string Name, string Value);

public sealed class Advertisement
{
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid RecordId { get; private init; }
    public long AdvertisementId { get; private init; }
    public string Title { get; private init; }
    public string Description { get; private init; }
    public long Price { get; private init; }
    public string PriceExtra { get; private init; }
    public string ServiceName { get; private init; }
    public string SourceUrl { get; private init; }
    public string Publisher { get; private init; }
    public string Address { get; private init; }
    public DateTime CreatedAt { get; private init; }
    public DateTime AdvertisementDate { get; private init; }
    public AdvertisementCharacteristic[] Characteristics { get; private init; }
    public string[] Photos { get; private init; }

    public Advertisement(
        long id,
        string title,
        string description,
        long price,
        string priceExtra,
        string serviceName,
        string sourceUrl,
        string publisher,
        string address,
        DateTime advertisementDate,
        AdvertisementCharacteristic[] characteristics,
        string[] photos
    ) =>
        (
            RecordId,
            AdvertisementId,
            Title,
            Description,
            Price,
            PriceExtra,
            ServiceName,
            SourceUrl,
            Publisher,
            Address,
            CreatedAt,
            AdvertisementDate,
            Characteristics,
            Photos
        ) = (
            Guid.NewGuid(),
            id,
            title,
            description,
            price,
            priceExtra,
            serviceName,
            sourceUrl,
            publisher,
            address,
            DateTime.Now.Date,
            advertisementDate,
            characteristics,
            photos
        );
}
