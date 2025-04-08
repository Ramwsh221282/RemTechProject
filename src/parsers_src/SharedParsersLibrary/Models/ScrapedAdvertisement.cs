using RemTech.Domain.AdvertisementsContext;
using RemTech.Domain.AdvertisementsContext.ValueObjects;
using RemTech.Shared.SDK.ResultPattern;

namespace SharedParsersLibrary.Models;

public sealed record ScrapedAdvertisement(
    long Id,
    string Address,
    string Description,
    long Price,
    string PriceExtra,
    string Publisher,
    string ServiceName,
    string SourceUrl,
    string Title,
    DateTime Published,
    ScrapedCharacteristic[] Characteristics,
    string[] PhotoUrls
)
{
    public static ScrapedAdvertisement Default() =>
        new(
            0,
            string.Empty,
            string.Empty,
            0,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            default,
            [],
            []
        );
}

public static class ScrapedAdvertisementExtensions
{
    public static Result<Advertisement> ToAdvertisemnet(this ScrapedAdvertisement advertisement)
    {
        Result<AdvertisementId> id = AdvertisementId.Dedicated(advertisement.Id);
        if (id.IsFailure)
            return id.Error;

        Result<AdvertisementTextInformation> text = AdvertisementTextInformation.Create(
            advertisement.Title,
            advertisement.Description
        );
        if (text.IsFailure)
            return text.Error;

        Result<AdvertisementPriceInformation> price = AdvertisementPriceInformation.Create(
            advertisement.PriceExtra,
            advertisement.Price
        );
        if (price.IsFailure)
            return price.Error;

        Result<AdvertisementScraperInformation> scraper = AdvertisementScraperInformation.Create(
            advertisement.SourceUrl,
            advertisement.ServiceName,
            advertisement.Publisher
        );
        if (scraper.IsFailure)
            return scraper.Error;

        Result<AdvertisementPhotoCollection> photos = advertisement.GetPhotos();
        if (photos.IsFailure)
            return photos.Error;

        Result<AdvertisementCharacteristicsCollection> ctx = advertisement.GetCharacteristics();
        if (ctx.IsFailure)
            return ctx.Error;

        return new Advertisement(id, text, price, scraper, photos, ctx);
    }

    private static Result<AdvertisementPhotoCollection> GetPhotos(
        this ScrapedAdvertisement advertisement
    )
    {
        Result<AdvertisementPhoto>[] photosResults =
        [
            .. advertisement.PhotoUrls.Select(AdvertisementPhoto.Create).Where(p => p.IsSuccess),
        ];
        AdvertisementPhoto[] photos = [.. photosResults.Select(p => p.Value)];
        Result<AdvertisementPhotoCollection> photoCollection = AdvertisementPhotoCollection.Create(
            photos
        );
        return photoCollection;
    }

    private static Result<AdvertisementCharacteristicsCollection> GetCharacteristics(
        this ScrapedAdvertisement advertisement
    )
    {
        Result<AdvertisementCharacteristic>[] ctxResults =
        [
            .. advertisement
                .Characteristics.Select(c => AdvertisementCharacteristic.Create(c.Name, c.Value))
                .Where(r => r.IsSuccess),
        ];
        AdvertisementCharacteristic[] ctx = [.. ctxResults.Select(c => c.Value)];
        return AdvertisementCharacteristicsCollection.Create(ctx);
    }
}
