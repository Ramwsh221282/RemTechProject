using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using RemTechAvito.Core.FiltersManagement.TransportTypes;
using RemTechAvito.Infrastructure.Repository.Common.Serializers;
using RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement;
using RemTechAvito.Infrastructure.Repository.TransportStatesFilterManagement.Serializers;
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
        try
        {
            var textSearchIndexModel = new CreateIndexModel<TransportType>(
                Builders<TransportType>.IndexKeys.Text("type_name"),
                new CreateIndexOptions() { Name = "Transport_Types_Text_Search" }
            );

            var textQueryIndexModel = new CreateIndexModel<TransportType>(
                Builders<TransportType>.IndexKeys.Ascending("type_name"),
                new CreateIndexOptions() { Name = "Transport_Types_Text_Query" }
            );

            var db = client.GetDatabase(TransportAdvertisementsRepository.DbName);
            var collection = db.GetCollection<TransportType>(Collection);
            await collection.Indexes.CreateManyAsync([textSearchIndexModel, textQueryIndexModel]);
        }
        catch
        {
            // ignored
        }
    }
}
