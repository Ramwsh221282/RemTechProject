using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using RemTechAvito.Core.FiltersManagement.TransportTypes;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement;
using RemTechAvito.Infrastructure.Repository.TransportTypesFilterManagement.Serializers;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using Exception = System.Exception;

namespace RemTechAvito.Infrastructure.Repository.TransportTypesFilterManagement;

public sealed class TransportTypesRepository(MongoClient client, ILogger logger)
    : ITransportTypesRepository
{
    private const string CollectionName = "Transport_Types";

    public async Task<Result> Add(TransportType type, CancellationToken ct = default)
    {
        try
        {
            var db = client.GetDatabase(TransportAdvertisementsRepository.DbName);
            await db.CreateCollectionAsync(CollectionName, cancellationToken: ct);
            var dbCollection = db.GetCollection<TransportType>(CollectionName);
            await dbCollection.InsertOneAsync(type, cancellationToken: ct);
            logger.Information("{Class} saved {Type}", nameof(TransportTypesRepository), type);
            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.Fatal(
                "{Class} cannot save {Type} exception: {Ex}",
                nameof(TransportTypesRepository),
                nameof(type),
                ex.Message
            );
            return new Error("Unable to save transport type collection. Internal error.");
        }
    }

    public static void RegisterSerializers()
    {
        BsonSerializer.RegisterSerializer(new TransportTypeSerializer());
    }
}
