using RemTechAvito.Contracts.Common.Dto.TransportAdvertisementsManagement;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement;

namespace RemTechAvito.Infrastructure.Contracts.Repository;

public interface ITransportAdvertisementsQueryRepository
{
    Task<IEnumerable<TransportAdvertisement>> Query(
        FilterAdvertisementsDto dto,
        CancellationToken ct = default
    );
}
