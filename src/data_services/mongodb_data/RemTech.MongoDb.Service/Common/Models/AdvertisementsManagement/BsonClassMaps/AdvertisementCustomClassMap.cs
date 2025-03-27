using RemTech.MongoDb.Service.Common.Abstractions.BsonClassMapping;
using RemTech.MongoDb.Service.Common.Models.AdvertisementsManagement.BsonSerializers;
using RemTech.MongoDb.Service.Common.Serializers;

namespace RemTech.MongoDb.Service.Common.Models.AdvertisementsManagement.BsonClassMaps;

public sealed class AdvertisementCustomClassMap : CustomClassMap<Advertisement>
{
    public AdvertisementCustomClassMap()
    {
        AutoMap();
        SetIgnoreExtraElements(true);
        MapIdProperty(ad => ad.RecordId);
        MapProperty(ad => ad.AdvertisementId);
        MapProperty(ad => ad.Title);
        MapProperty(ad => ad.Description);
        MapProperty(ad => ad.Price);
        MapProperty(ad => ad.PriceExtra);
        MapProperty(ad => ad.ServiceName);
        MapProperty(ad => ad.SourceUrl);
        MapProperty(ad => ad.Publisher);
        MapProperty(ad => ad.Address);
        MapProperty(ad => ad.CreatedAt).SetSerializer(new DateTimeSerializer());
        MapProperty(ad => ad.AdvertisementDate).SetSerializer(new DateTimeSerializer());
        MapProperty(ad => ad.Characteristics)
            .SetSerializer(new AdvertisementCharacteristicsSerializer());
        MapProperty(ad => ad.Photos).SetSerializer(new StringArraySerializer());
    }
}
