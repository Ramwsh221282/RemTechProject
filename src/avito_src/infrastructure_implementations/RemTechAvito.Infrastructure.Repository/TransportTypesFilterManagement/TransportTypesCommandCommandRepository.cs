using MongoDB.Driver;
using RemTechAvito.Contracts.Common.Dto.TransportTypesManagement;
using RemTechAvito.Core.FiltersManagement.TransportTypes;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement;
using RemTechAvito.Infrastructure.Repository.TransportTypesFilterManagement.Extensions;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using Exception = System.Exception;

namespace RemTechAvito.Infrastructure.Repository.TransportTypesFilterManagement;

internal sealed class TransportTypesCommandCommandRepository(MongoClient client, ILogger logger)
    : ITransportTypesCommandRepository
{
    public async Task<Result> AddMany(
        IEnumerable<TransportType> types,
        CancellationToken ct = default
    )
    {
        try
        {
            var db = client.GetDatabase(TransportAdvertisementsRepository.DbName);
            var collection = db.GetCollection<TransportType>(TransportTypesMetadata.Collection);
            await collection.InsertManyAsync(types, cancellationToken: ct);
            logger.Information(
                "{Class} saved transport types",
                nameof(TransportTypesCommandCommandRepository)
            );
            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.Fatal(
                "{Class} exception {Ex} on method {Method}",
                nameof(TransportTypesCommandCommandRepository),
                ex.Message,
                nameof(AddMany)
            );
            return new Error("Unable to save transport type collection. Internal error.");
        }
    }

    public async Task<Result> Add(TransportType type, CancellationToken ct = default)
    {
        try
        {
            var db = client.GetDatabase(TransportAdvertisementsRepository.DbName);
            var collection = db.GetCollection<TransportType>(TransportTypesMetadata.Collection);
            await collection.InsertOneAsync(type, cancellationToken: ct);
            logger.Information(
                "{Class} added new transport type: {Type}",
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
            return new Error("Unable to save transport type. Internal error.");
        }
    }

    public async Task<Result> Delete(RemoveTransportTypeQuery query, CancellationToken ct = default)
    {
        try
        {
            var filterDefinition = query.CreateFilter();
            var db = client.GetDatabase(TransportAdvertisementsRepository.DbName);
            var collection = db.GetCollection<TransportType>(TransportTypesMetadata.Collection);
            var deleteResult = await collection.DeleteManyAsync(filterDefinition, ct);

            if (!deleteResult.IsAcknowledged)
            {
                logger.Error(
                    "{Class} unable to delete transport types.",
                    nameof(TransportTypesCommandCommandRepository)
                );

                return new Error("Unable to delete transport type.");
            }

            if (deleteResult.DeletedCount == 0)
            {
                logger.Error(
                    "{Class} unable to delete transport type.",
                    nameof(TransportTypesCommandCommandRepository)
                );

                return new Error("Unable to delete transport type.");
            }

            logger.Information(
                "{Class} removed {Count} transport types.",
                nameof(TransportTypesCommandCommandRepository),
                deleteResult.DeletedCount
            );
            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.Fatal(
                "{Class} exception {Ex} on method {Method}",
                nameof(TransportTypesCommandCommandRepository),
                ex.Message,
                nameof(Delete)
            );
            return new Error("Unable to remove transport type. Internal error.");
        }
    }
}
