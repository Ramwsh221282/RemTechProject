using RemTech.Infrastructure.PostgreSql.AdvertisementsContext.DaoModels;

namespace RemTech.WebApi.AdvertisementsContext.Responses;

public sealed record AdvertisementResponse(
    long Id,
    string PriceExtra,
    long PriceValue,
    string PublishedBy,
    string ScraperName,
    string SourceUrl,
    string Description,
    string Title,
    string Address,
    AdvertisementCharacteristicResponse[] Characteristics,
    AdvertisementPhotoResponse[] Photos
);

public sealed record AdvertisementCharacteristicResponse(string Name, string Value);

public sealed record AdvertisementPhotoResponse(string SourceUrl);

public static class AdvertisementResponseExtensions
{
    public static AdvertisementResponse FromDao(this AdvertisementDao daoModel)
    {
        AdvertisementResponse response = new(
            daoModel.Id,
            daoModel.PriceExtra,
            daoModel.PriceValue,
            daoModel.PublishedBy,
            daoModel.ScraperName,
            daoModel.SourceUrl,
            daoModel.Description,
            daoModel.Title,
            daoModel.Address,
            daoModel.Characteristics.FromDao(),
            daoModel.Photos.FromDao()
        );

        return response;
    }

    public static AdvertisementCharacteristicResponse FromDao(
        this AdvertisementCharacteristicDao daoModel
    ) => new(daoModel.Name, daoModel.Value);

    private static AdvertisementCharacteristicResponse[] FromDao(
        this AdvertisementsCharacteristicsDao ctxDao
    )
    {
        AdvertisementCharacteristicResponse[] response =
        [
            .. ctxDao.Select(c => new AdvertisementCharacteristicResponse(c.Name, c.Value)),
        ];

        return response;
    }

    private static AdvertisementPhotoResponse[] FromDao(this AdvertisementPhotosDao photosDao)
    {
        AdvertisementPhotoResponse[] response =
        [
            .. photosDao.Select(p => new AdvertisementPhotoResponse(p.SourceUrl)),
        ];

        return response;
    }
}
