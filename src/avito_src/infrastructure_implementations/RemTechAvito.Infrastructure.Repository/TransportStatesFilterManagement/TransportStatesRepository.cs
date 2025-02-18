using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using RemTechAvito.Core.FiltersManagement.TransportStates;
using RemTechAvito.Core.FiltersManagement.TransportTypes;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement;
using RemTechAvito.Infrastructure.Repository.TransportTypesFilterManagement;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTechAvito.Infrastructure.Repository.TransportStatesFilterManagement;

internal sealed class TransportStatesRepository(MongoClient client, ILogger logger)
    : ITransportStatesRepository
{
    private const string CollectionName = "Transport_States";

    public async Task<Result> Add(TransportState state, CancellationToken ct = default)
    {
        try
        {
            var db = client.GetDatabase(TransportAdvertisementsRepository.DbName);
            await db.CreateCollectionAsync(CollectionName, cancellationToken: ct);
            var collection = db.GetCollection<TransportState>(CollectionName);
            await collection.InsertOneAsync(state, cancellationToken: ct);
            logger.Information(
                "{Repository} method saved {State}.",
                nameof(TransportTypesRepository),
                state
            );
            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.Error(
                "{Repository} cannot save {State} exception: {Ex}.",
                nameof(TransportTypesRepository),
                state,
                ex.Message
            );
            return new Error("Internal error. Cannot save transport state.");
        }
    }
}
