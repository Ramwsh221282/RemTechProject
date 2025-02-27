using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using RemTechAvito.Core.FiltersManagement.TransportTypes;
using RemTechAvito.Infrastructure.Repository.Common.Indexes;
using RemTechAvito.Infrastructure.Repository.Common.Serializers;
using RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement;
using RemTechAvito.Infrastructure.Repository.TransportStatesFilterManagement.Serializers;
using RemTechAvito.Infrastructure.Repository.TransportTypesFilterManagement.Indexes;
using RemTechAvito.Infrastructure.Repository.TransportTypesFilterManagement.Serializers;

namespace RemTechAvito.Infrastructure.Repository.TransportTypesFilterManagement;

internal static class TransportTypesMetadata
{
    internal const string Collection = "Transport_Types";

    public static void RegisterMetadata()
    {
        RegisterSerializers();
        RegisterClassMaps();
    }

    private static void RegisterSerializers()
    {
        BsonSerializer.RegisterSerializer(new TransportStateSerializer());
        BsonSerializer.RegisterSerializer(new TransportTypeSerializer());
        BsonSerializer.RegisterSerializer(new DateOnlySerializer());
    }

    private static void RegisterClassMaps()
    {
        BsonClassMap.RegisterClassMap(new TransportTypesBsonClassMap());
    }

    public static async Task RegisterIndexes(MongoClient client)
    {
        AbstractIndexModel<TransportType> model = new TransportTypesIndexModel();
        await model.UpdateIndexesAsync(
            client,
            Collection,
            TransportAdvertisementsRepository.DbName
        );
    }
}
