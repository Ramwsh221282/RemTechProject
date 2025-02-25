using RemTechAvito.Contracts.Common.Dto.TransportTypesManagement;
using RemTechAvito.Contracts.Common.Responses.TransportTypesManagement;

namespace RemTechAvito.Infrastructure.Contracts.Repository;

public interface ITransportTypesQueryRepository
{
    Task<TransportTypeResponse> Get(CancellationToken ct = default);
    Task<TransportTypeResponse> Get(GetTransportTypesQuery query, CancellationToken ct = default);
}
