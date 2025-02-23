using MongoDB.Bson.Serialization;
using RemTechAvito.Infrastructure.Repository.ParserProfileManagement.Mappers;
using RemTechAvito.Infrastructure.Repository.ParserProfileManagement.Serializers;

namespace RemTechAvito.Infrastructure.Repository.ParserProfileManagement;

internal sealed class ParserProfileMetadata
{
    internal const string CollectionName = "parser_profile";

    public static void RegisterParserProfileMetadata()
    {
        RegisterSerializers();
        RegisterClassMaps();
    }

    private static void RegisterSerializers()
    {
        BsonSerializer.RegisterSerializer(new ParserProfileIdSerializer());
        BsonSerializer.RegisterSerializer(new ParserProfileLinkBodySerializer());
        BsonSerializer.RegisterSerializer(new ParserProfileLinkIdSerializer());
        BsonSerializer.RegisterSerializer(new ParserProfileLinksCollectionSerializer());
        BsonSerializer.RegisterSerializer(new ParserProfileStateSerializer());
    }

    private static void RegisterClassMaps()
    {
        BsonClassMap.RegisterClassMap(new ParserProfileClassMap());
        BsonClassMap.RegisterClassMap(new ParserProfileLinkMapper());
    }
}
