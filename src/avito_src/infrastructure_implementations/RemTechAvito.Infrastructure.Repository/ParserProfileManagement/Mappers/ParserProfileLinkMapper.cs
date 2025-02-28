using MongoDB.Bson.Serialization;
using RemTechAvito.Core.ParserProfileManagement.ValueObjects;

namespace RemTechAvito.Infrastructure.Repository.ParserProfileManagement.Mappers;

internal sealed class ParserProfileLinkMapper : BsonClassMap<ParserProfileLink>
{
    public ParserProfileLinkMapper()
    {
        AutoMap();
        SetIgnoreExtraElements(true);
        MapMember(l => l.Link);
        MapMember(l => l.Name);
    }
}
