namespace RemTech.MongoDb.Service.Features.AdvertisementsManagement.AdvertisementQuerying;

public sealed record TransportAdvertisement(
    long AdvertisementId,
    string Title,
    string Description,
    long Price,
    string PriceExtra,
    string ServiceName,
    string SourceUrl,
    string Publisher,
    string Address,
    DateTime CreatedAt,
    DateTime AdvertisementDate,
    TransportAdvertisementCharacteristic[] Characteristics,
    string[] Photos
);
