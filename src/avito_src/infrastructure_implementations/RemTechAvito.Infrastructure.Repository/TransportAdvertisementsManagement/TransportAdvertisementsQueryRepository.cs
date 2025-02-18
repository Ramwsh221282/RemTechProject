using MongoDB.Driver;
using RemTechAvito.Contracts.Common.Dto.TransportAdvertisementsManagement;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement.Queries;

namespace RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement;

internal sealed class TransportAdvertisementsQueryRepository(
    MongoClient client,
    TransportAdvertisementsQueryResolver resolver
) : ITransportAdvertisementsQueryRepository
{
    public async Task<IEnumerable<TransportAdvertisement>> Query(
        FilterAdvertisementsDto dto,
        CancellationToken ct = default
    )
    {
        var filter = resolver.Resolve(dto);
        var db = client.GetDatabase(TransportAdvertisementsRepository.DbName);
        var collection = db.GetCollection<TransportAdvertisement>(
            TransportAdvertisementsRepository.CollectionName
        );

        var tempCursor = await collection.FindAsync(filter, cancellationToken: ct);
        return await tempCursor.ToListAsync(cancellationToken: ct);
    }
}
