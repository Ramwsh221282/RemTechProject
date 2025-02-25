using MongoDB.Driver;
using RemTechAvito.Contracts.Common.Dto.TransportTypesManagement;
using RemTechAvito.Contracts.Common.Responses.TransportTypesManagement;
using RemTechAvito.Core.FiltersManagement.TransportTypes;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement;
using RemTechCommon.Utils.Extensions;
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
        var db = client.GetDatabase(TransportAdvertisementsRepository.DbName);
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
        var sortOrder = query.Sort?.Mode;
        var sorting = sortOrder switch
        {
            "ASC" => Builders<TransportType>.Sort.Ascending("type_name"),
            "DESC" => Builders<TransportType>.Sort.Descending("type_name"),
            _ => null,
        };

        var textSearch = query.TextSearch?.Text;
        FilterDefinition<TransportType> filter = textSearch switch
        {
            not null => Builders<TransportType>.Filter.Text(
                textSearch.CleanString(),
                new TextSearchOptions() { CaseSensitive = false, DiacriticSensitive = false }
            ),
            _ => Builders<TransportType>.Filter.Empty,
        };

        var db = client.GetDatabase(TransportAdvertisementsRepository.DbName);
        var collection = db.GetCollection<TransportType>(TransportTypesMetadata.Collection);

        var findQuery = collection.Find(filter);

        if (sorting != null)
            findQuery.Sort(sorting);

        var countTask = findQuery.CountDocumentsAsync(ct);
        var fetchTask = findQuery
            .Skip((query.Pagination.Page - 1) * query.Pagination.PageSize)
            .Limit(query.Pagination.PageSize)
            .ToListAsync(ct);

        await Task.WhenAll(countTask, fetchTask);

        var count = countTask.Result;
        var fetchedData = fetchTask.Result;

        IEnumerable<TransportTypeDto> response = fetchedData.Select(t => new TransportTypeDto(
            t.Name,
            t.Link
        ));
        return new TransportTypeResponse(response, count);
    }
}
