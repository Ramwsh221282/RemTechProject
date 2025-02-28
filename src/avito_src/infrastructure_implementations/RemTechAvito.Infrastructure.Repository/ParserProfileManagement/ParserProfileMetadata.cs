using MongoDB.Bson.Serialization;
using RemTechAvito.Infrastructure.Repository.ParserProfileManagement.Mappers;
using RemTechAvito.Infrastructure.Repository.ParserProfileManagement.Serializers;

namespace RemTechAvito.Infrastructure.Repository.ParserProfileManagement;

internal static class ParserProfileMetadata
{
    internal const string CollectionName = "Parser_Profiles";

    public static void RegisterMetadata()
    {
        RegisterSerializers();
        RegisterClassMaps();
    }

    private static void RegisterSerializers()
    {
        BsonSerializer.RegisterSerializer(new ParserProfileIdSerializer());
        BsonSerializer.RegisterSerializer(new ParserProfileLinksCollectionSerializer());
        BsonSerializer.RegisterSerializer(new ParserProfileStateSerializer());
    }

    private static void RegisterClassMaps()
    {
        BsonClassMap.RegisterClassMap(new ParserProfileClassMap());
        BsonClassMap.RegisterClassMap(new ParserProfileLinkMapper());
    }
}
