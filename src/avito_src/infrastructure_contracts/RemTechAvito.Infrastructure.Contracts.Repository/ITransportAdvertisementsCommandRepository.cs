using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement;

namespace RemTechAvito.Infrastructure.Contracts.Repository;

public interface ITransportAdvertisementsCommandRepository
{
    Task<Guid> Add(TransportAdvertisement advertisement, CancellationToken ct = default);
}
