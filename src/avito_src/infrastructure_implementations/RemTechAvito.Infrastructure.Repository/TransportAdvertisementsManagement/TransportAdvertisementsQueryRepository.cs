using MongoDB.Driver;
using RemTechAvito.Contracts.Common.Dto.TransportAdvertisementsManagement;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement.Specifications;

namespace RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement;

internal sealed class TransportAdvertisementsQueryRepository(MongoClient client)
    : ITransportAdvertisementsQueryRepository
{
    public async Task<IEnumerable<TransportAdvertisement>> Query(
        FilterAdvertisementsDto dto,
        CancellationToken ct = default
    )
    {
        MongoAdvertisementSpecification specification = new MongoAdvertisementSpecification(dto);
        var filter = specification.ToFilterDefinition();
        var db = client.GetDatabase(TransportAdvertisementsRepository.DbName);
        var collection = db.GetCollection<TransportAdvertisement>(
            TransportAdvertisementsRepository.CollectionName
        );
        var tempCursor = await collection.FindAsync(filter, cancellationToken: ct);
        return tempCursor.ToEnumerable(cancellationToken: ct);
    }
}
