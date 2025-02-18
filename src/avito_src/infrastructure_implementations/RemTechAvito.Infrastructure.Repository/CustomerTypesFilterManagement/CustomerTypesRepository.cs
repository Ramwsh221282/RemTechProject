using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using RemTechAvito.Core.FiltersManagement.CustomerTypes;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechAvito.Infrastructure.Repository.CustomerTypesFilterManagement.Serializers;
using RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTechAvito.Infrastructure.Repository.CustomerTypesFilterManagement;

internal sealed class CustomerTypesRepository(MongoClient client, ILogger logger)
    : ICustomerTypesRepository
{
    private const string CollectionName = "Customer_Types";

    public async Task<Result> Add(CustomerType type, CancellationToken ct = default)
    {
        try
        {
            var db = client.GetDatabase(TransportAdvertisementsRepository.DbName);
            await db.CreateCollectionAsync(CollectionName, cancellationToken: ct);
            var collection = db.GetCollection<CustomerType>(CollectionName);
            await collection.InsertOneAsync(type, cancellationToken: ct);
            logger.Information("{Class} added {Type}", nameof(CustomerTypesRepository), type);
            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.Error(
                "{Class} cannot save {Type}. Exception: {Ex}",
                nameof(CustomerTypesRepository),
                type,
                ex.Message
            );
            return new Error("Cannot save customer type. Internal Error");
        }
    }
}
