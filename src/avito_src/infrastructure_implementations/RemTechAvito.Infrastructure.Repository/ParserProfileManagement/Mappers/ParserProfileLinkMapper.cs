using MongoDB.Bson.Serialization;
using RemTechAvito.Core.ParserProfileManagement.Entities.ParserProfileLinks;
using RemTechAvito.Infrastructure.Repository.ParserProfileManagement.Serializers;

namespace RemTechAvito.Infrastructure.Repository.ParserProfileManagement.Mappers;

internal sealed class ParserProfileLinkMapper : BsonClassMap<ParserProfileLink>
{
    public ParserProfileLinkMapper()
    {
        AutoMap();
        SetIgnoreExtraElements(true);
        MapIdProperty(x => x.Id).SetSerializer(new ParserProfileLinkIdSerializer());
        MapMember(x => x.Body).SetSerializer(new ParserProfileLinkBodySerializer());
    }
}
