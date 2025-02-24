using RemTechAvito.Core.FiltersManagement.TransportTypes;
using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Infrastructure.Contracts.Repository;

public interface ITransportTypesCommandRepository
{
    Task<Result> Add(TransportType type, CancellationToken ct = default);
}
