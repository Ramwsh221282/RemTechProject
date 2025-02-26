using MongoDB.Bson.Serialization;
using RemTechAvito.Infrastructure.Repository.ParserJournalsManagement.Mappers;
using RemTechAvito.Infrastructure.Repository.ParserJournalsManagement.Serializers;

namespace RemTechAvito.Infrastructure.Repository.ParserJournalsManagement;

internal static class ParserJournalMetadata
{
    internal const string DbName = "Parser_Journals_Db";
    internal const string CollectionName = "Parser_Journals";

    public static void RegisterMetadata()
    {
        RegisterSerializers();
        RegisterClassMap();
    }

    private static void RegisterSerializers()
    {
        BsonSerializer.RegisterSerializer(new TimeSerializer());
    }

    private static void RegisterClassMap()
    {
        BsonClassMap.RegisterClassMap(new ParserJournalClassMap());
    }
}
