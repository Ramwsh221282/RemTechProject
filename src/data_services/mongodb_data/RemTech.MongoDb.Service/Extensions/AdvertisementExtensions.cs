using RemTech.MongoDb.Service.Common.Models.AdvertisementsManagement;
using RemTech.MongoDb.Service.Features.AdvertisementsManagement.AdvertisementQuerying;

namespace RemTech.MongoDb.Service.Extensions;

public static class AdvertisementExtensions
{
    public static TransportAdvertisement ToTransportAdvertisement(
        this Advertisement advertisement
    ) =>
        new(
            advertisement.AdvertisementId,
            advertisement.Title,
            advertisement.Description,
            advertisement.Price,
            advertisement.PriceExtra,
            advertisement.ServiceName,
            advertisement.SourceUrl,
            advertisement.Publisher,
            advertisement.Address,
            advertisement.CreatedAt,
            advertisement.AdvertisementDate,
            [.. advertisement.Characteristics.Select(TransportAdvertisementCharacteristic)],
            advertisement.Photos
        );

    private static TransportAdvertisementCharacteristic TransportAdvertisementCharacteristic(
        this AdvertisementCharacteristic characteristic
    ) => new(characteristic.Name, characteristic.Value);
}
