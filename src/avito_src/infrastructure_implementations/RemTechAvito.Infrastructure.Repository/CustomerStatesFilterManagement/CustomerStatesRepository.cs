using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using RemTechAvito.Core.FiltersManagement.CustomerStates;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechAvito.Infrastructure.Repository.CustomerStatesFilterManagement.Serializers;
using RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTechAvito.Infrastructure.Repository.CustomerStatesFilterManagement;

public sealed class CustomerStatesRepository(MongoClient client, ILogger logger)
    : ICustomerStatesRepository
{
    private const string CollectionName = "Customer_States";

    public async Task<Result> Add(CustomerState state, CancellationToken ct = default)
    {
        try
        {
            var db = client.GetDatabase(TransportAdvertisementsRepository.DbName);
            await db.CreateCollectionAsync(CollectionName, cancellationToken: ct);
            var collection = db.GetCollection<CustomerState>(CollectionName);
            await collection.InsertOneAsync(state, cancellationToken: ct);
            logger.Information("{Class} added {State}", nameof(CustomerStatesRepository), state);
            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.Error(
                "{Class} cannot add {State}. Error: {ex}",
                nameof(CustomerStatesRepository),
                state,
                ex.Message
            );
            return new Error("Cannot add customer state. Internal error.");
        }
    }

    public static void RegisterSerializer()
    {
        BsonSerializer.RegisterSerializer(new CustomerStateSerializer());
    }
}
