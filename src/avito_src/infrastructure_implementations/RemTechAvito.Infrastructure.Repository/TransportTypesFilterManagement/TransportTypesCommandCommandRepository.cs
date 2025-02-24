using MongoDB.Driver;
using RemTechAvito.Core.FiltersManagement.TransportTypes;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using Exception = System.Exception;

namespace RemTechAvito.Infrastructure.Repository.TransportTypesFilterManagement;

internal sealed class TransportTypesCommandCommandRepository(MongoClient client, ILogger logger)
    : ITransportTypesCommandRepository
{
    public async Task<Result> Add(TransportType type, CancellationToken ct = default)
    {
        try
        {
            var db = client.GetDatabase(TransportAdvertisementsRepository.DbName);
            await db.CreateCollectionAsync(
                TransportTypesMetadata.Collection,
                cancellationToken: ct
            );
            var dbCollection = db.GetCollection<TransportType>(TransportTypesMetadata.Collection);
            await dbCollection.InsertOneAsync(type, cancellationToken: ct);
            logger.Information(
                "{Class} saved {Type}",
                nameof(TransportTypesCommandCommandRepository),
                type
            );
            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.Fatal(
                "{Class} exception {Ex} on method {Method}",
                nameof(TransportTypesCommandCommandRepository),
                ex.Message,
                nameof(Add)
            );
            return new Error("Unable to save transport type collection. Internal error.");
        }
    }
}
