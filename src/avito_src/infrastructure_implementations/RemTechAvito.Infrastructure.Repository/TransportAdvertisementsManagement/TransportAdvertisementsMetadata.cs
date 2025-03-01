using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement;
using RemTechAvito.Infrastructure.Repository.Common.Indexes;
using RemTechAvito.Infrastructure.Repository.Common.Serializers;
using RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement.Indexes;
using RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement.Serializers;

namespace RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement;

internal static class TransportAdvertisementsMetadata
{
    public const string Collection = "Parsed_Advertisements";

    public static void RegisterMetadata()
    {
        RegisterSerializers();
        RegisterBsonClassMaps();
    }

    private static void RegisterSerializers()
    {
        BsonSerializer.RegisterSerializer(new AddressSerializer());
        BsonSerializer.RegisterSerializer(new AdvertisementIDSerializer());
        BsonSerializer.RegisterSerializer(new CharacteristicsSerializer());
        BsonSerializer.RegisterSerializer(new DescriptionSerializer());
        BsonSerializer.RegisterSerializer(new EntityIdSerializer());
        BsonSerializer.RegisterSerializer(new OwnerInformationSerializer());
        BsonSerializer.RegisterSerializer(new PhotoAttachmentsSerializer());
        BsonSerializer.RegisterSerializer(new PriceSerializer());
        BsonSerializer.RegisterSerializer(new TitleSerializer());
        BsonSerializer.RegisterSerializer(new AdvertisementUrlSerializer());
    }

    private static void RegisterBsonClassMaps()
    {
        BsonClassMap.RegisterClassMap(new TransportAdvertisementClassMap());
    }

    public static async Task RegisterIndexes(MongoClient client)
    {
        AbstractIndexModel<TransportAdvertisement> model = new TransportAdvertisementsIndexModel();
        await model.UpdateIndexesAsync(client, Collection, MongoDbOptions.Databse);
    }
}
