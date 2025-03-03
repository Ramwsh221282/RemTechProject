using RemTechAvito.Contracts.Common.Dto.TransportTypesManagement;
using RemTechAvito.Core.AdvertisementManagement.TransportTypes;
using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Infrastructure.Contracts.Repository;

public interface ITransportTypesCommandRepository
{
    Task<Result> AddMany(IEnumerable<TransportType> types, CancellationToken ct = default);
    Task<Result> Add(TransportType type, CancellationToken ct = default);
    Task<Result> Delete(RemoveTransportTypeQuery query, CancellationToken ct = default);
}
