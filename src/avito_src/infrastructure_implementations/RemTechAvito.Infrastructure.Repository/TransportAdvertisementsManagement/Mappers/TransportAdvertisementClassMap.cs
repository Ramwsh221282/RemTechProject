using MongoDB.Bson.Serialization;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement;
using RemTechAvito.Infrastructure.Repository.Common.Serializers;
using RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement.Serializers;

namespace RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement.Mappers;

internal sealed class TransportAdvertisementClassMap : BsonClassMap<TransportAdvertisement>
{
    public TransportAdvertisementClassMap()
    {
        AutoMap();
        SetIgnoreExtraElements(true);
        MapIdProperty(x => x.TransportAdvertisementId).SetSerializer(new EntityIdSerializer());
        MapMember(x => x.AdvertisementId).SetSerializer(new AdvertisementIDSerializer());
        MapMember(x => x.Characteristics).SetSerializer(new CharacteristicsSerializer());
        MapMember(x => x.Address).SetSerializer(new AddressSerializer());
        MapMember(x => x.OwnerInformation).SetSerializer(new OwnerInformationSerializer());
        MapMember(x => x.PhotoAttachments).SetSerializer(new PhotoAttachmentsSerializer());
        MapMember(x => x.Price).SetSerializer(new PriceSerializer());
        MapMember(x => x.Title).SetSerializer(new TitleSerializer());
        MapMember(x => x.Description).SetSerializer(new DescriptionSerializer());
        MapMember(x => x.Url).SetSerializer(new AdvertisementUrlSerializer());
        MapMember(x => x.CreatedOn).SetSerializer(new DateOnlySerializer());
    }
}
