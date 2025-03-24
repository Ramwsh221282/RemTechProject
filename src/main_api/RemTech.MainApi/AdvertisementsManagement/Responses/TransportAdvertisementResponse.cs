using RemTech.MainApi.AdvertisementsManagement.Models;

namespace RemTech.MainApi.AdvertisementsManagement.Responses;

public sealed record TransportAdvertisementResponse(
    TransportAdvertisementResponseItem[] Items,
    long Count
);

public sealed record TransportAdvertisementResponseItem(
    long Id,
    string Title,
    string Description,
    string Address,
    string ServiceName,
    string SourceUrl,
    long Price,
    string PriceExtra,
    string[] Photos,
    TransportAdvertisementCharacteristicResponse[] Characteristics
);

public static class TransportAdvertisementResponseExtensions
{
    public static TransportAdvertisementResponseItem ToResponse(
        this TransportAdvertisement advertisement
    ) =>
        new(
            advertisement.AdvertisementId,
            advertisement.Title,
            advertisement.Description,
            advertisement.Address,
            advertisement.ServiceName,
            advertisement.SourceUrl,
            advertisement.Price,
            advertisement.PriceExtra,
            advertisement.Photos,
            [.. advertisement.Characteristics.Select(ToResponse)]
        );

    public static TransportAdvertisementCharacteristicResponse ToResponse(
        this TransportAdvertisementCharacteristic characteristic
    ) => new(characteristic.Name, characteristic.Value);
}
