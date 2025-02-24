using RemTechAvito.Contracts.Common.Dto.TransportTypesManagement;
using RemTechAvito.Contracts.Common.Responses.TransportTypesManagement;

namespace RemTechAvito.Infrastructure.Contracts.Repository;

public interface ITransportTypesQueryRepository
{
    Task<IEnumerable<TransportTypeResponse>> Get(CancellationToken ct = default);

    Task<IEnumerable<TransportTypeResponse>> Get(
        GetTransportTypesQuery query,
        CancellationToken ct = default
    );
}
