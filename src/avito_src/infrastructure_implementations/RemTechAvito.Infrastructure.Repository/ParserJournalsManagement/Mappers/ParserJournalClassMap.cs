using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using RemTechAvito.Core.ParserJournalManagement;
using RemTechAvito.Infrastructure.Repository.ParserJournalsManagement.Serializers;

namespace RemTechAvito.Infrastructure.Repository.ParserJournalsManagement.Mappers;

internal sealed class ParserJournalClassMap : BsonClassMap<ParserJournal>
{
    public ParserJournalClassMap()
    {
        AutoMap();
        SetIgnoreExtraElements(true);
        MapIdProperty(x => x.Id).SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
        MapMember(x => x.Time).SetSerializer(new TimeSerializer());
        MapMember(x => x.IsSuccess);
        MapMember(x => x.ErrorMessage);
        MapMember(x => x.ItemsParsed);
        MapMember(x => x.Description);
        MapMember(x => x.Source);
        MapMember(x => x.CreatedOn);
    }
}
