using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement;

namespace RemTechAvito.Infrastructure.Contracts.Repository;

public interface ITransportAdvertisementsRepository
{
    Task<Guid> Add(TransportAdvertisement advertisement, CancellationToken ct = default);
}
