using RemTech.MongoDb.Service.Features.AdvertisementsSinking;

namespace RemTech.MongoDb.Service.Common.Models.AdvertisementsManagement.Converters;

public sealed class AdvertisementsFactory
{
    public Advertisement FromSinkedAdvertisement(SinkingAdvertisement advertisement)
    {
        return new Advertisement(
            advertisement.AdvertisementId,
            advertisement.Title,
            advertisement.Description,
            advertisement.Price,
            advertisement.PriceExtra,
            advertisement.FromParser,
            advertisement.SourceUrl,
            advertisement.Publisher,
            advertisement.Address,
            advertisement.Published,
            advertisement.Characteristics.Select(c => FromSinkedAdvertisement(c)).ToArray(),
            advertisement.PhotoUrls
        );
    }

    public AdvertisementCharacteristic FromSinkedAdvertisement(
        SinkingCharacteristics characteristics
    ) => new(characteristics.Name, characteristics.Value);
}
