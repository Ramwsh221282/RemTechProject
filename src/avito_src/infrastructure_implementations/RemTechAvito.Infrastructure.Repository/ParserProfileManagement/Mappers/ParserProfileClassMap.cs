using MongoDB.Bson.Serialization;
using RemTechAvito.Core.ParserProfileManagement;
using RemTechAvito.Infrastructure.Repository.ParserProfileManagement.Serializers;
using RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement.Serializers;

namespace RemTechAvito.Infrastructure.Repository.ParserProfileManagement.Mappers;

internal sealed class ParserProfileClassMap : BsonClassMap<ParserProfile>
{
    public ParserProfileClassMap()
    {
        AutoMap();
        SetIgnoreExtraElements(true);
        MapIdProperty(x => x.Id).SetSerializer(new ParserProfileIdSerializer());
        MapMember(x => x.CreatedOn).SetSerializer(new DateCreatedSerializer());
        MapMember(x => x.State).SetSerializer(new ParserProfileStateSerializer());
        MapMember(x => x.Links).SetSerializer(new ParserProfileLinksCollectionSerializer());
    }
}
