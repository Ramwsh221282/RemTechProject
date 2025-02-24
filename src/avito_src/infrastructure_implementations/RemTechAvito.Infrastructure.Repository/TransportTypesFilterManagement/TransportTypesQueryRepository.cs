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
    public async Task<IEnumerable<TransportTypeResponse>> Get(CancellationToken ct = default)
    {
        logger.Information(
            "{Repository} transport types requested",
            nameof(TransportTypesQueryRepository)
        );
        var db = client.GetDatabase(TransportAdvertisementsRepository.DbName);
        var collection = db.GetCollection<TransportType>(TransportTypesMetadata.Collection);
        var cursor = await collection.FindAsync(_ => true, cancellationToken: ct);
        var items = await cursor.ToListAsync(ct);
        return items.Select(i => new TransportTypeResponse(i.Name, i.Link)).ToArray();
    }

    public async Task<IEnumerable<TransportTypeResponse>> Get(
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

        IFindFluent<TransportType, TransportType> findQuery = collection.Find(filter);

        if (sorting != null)
            findQuery.Sort(sorting);

        var items = await findQuery
            .Skip((query.Pagination.Page - 1) * query.Pagination.PageSize)
            .Limit(query.Pagination.PageSize)
            .ToListAsync(ct);

        return items == null ? [] : items.Select(i => new TransportTypeResponse(i.Name, i.Link));
    }
}
