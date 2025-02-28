using MongoDB.Driver;
using RemTechAvito.Contracts.Common.Dto.TransportTypesManagement;
using RemTechAvito.Contracts.Common.Responses.TransportTypesManagement;
using RemTechAvito.Core.AdvertisementManagement.TransportTypes;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement;
using RemTechAvito.Infrastructure.Repository.TransportTypesFilterManagement.Models;
using Serilog;

namespace RemTechAvito.Infrastructure.Repository.TransportTypesFilterManagement;

internal sealed class TransportTypesQueryRepository(MongoClient client, ILogger logger)
    : ITransportTypesQueryRepository
{
    public async Task<TransportTypeResponse> Get(CancellationToken ct = default)
    {
        logger.Information(
            "{Repository} transport types requested",
            nameof(TransportTypesQueryRepository)
        );
        var db = client.GetDatabase(MongoDbOptions.Databse);
        var collection = db.GetCollection<TransportType>(TransportTypesMetadata.Collection);
        var cursor = await collection.FindAsync(_ => true, cancellationToken: ct);
        var items = await cursor.ToListAsync(ct);
        var response = items.Select(t => new TransportTypeDto(t.Name, t.Link));
        return new TransportTypeResponse(response, items.Count);
    }

    public async Task<TransportTypeResponse> Get(
        GetTransportTypesQuery query,
        CancellationToken ct = default
    )
    {
        var queryModel = GetTransportTypeQueryModelExtensions.Create(query);
        var db = client.GetDatabase(MongoDbOptions.Databse);
        var collection = db.GetCollection<TransportType>(TransportTypesMetadata.Collection);
        var findQuery = collection.Find(queryModel.Filter);
        if (queryModel.Sort != null)
            findQuery.Sort(queryModel.Sort);

        var countTask = findQuery.CountDocumentsAsync(ct);
        var fetchTask = findQuery
            .Skip((queryModel.Page - 1) * queryModel.Size)
            .Limit(queryModel.Size)
            .ToListAsync(ct);
        await Task.WhenAll(countTask, fetchTask);

        var count = countTask.Result;
        var fetchedData = fetchTask.Result;

        var response = fetchedData.Select(t => new TransportTypeDto(t.Name, t.Link));
        return new TransportTypeResponse(response, count);
    }
}
