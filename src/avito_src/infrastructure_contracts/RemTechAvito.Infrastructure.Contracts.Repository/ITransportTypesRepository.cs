using RemTechAvito.Core.FiltersManagement.TransportTypes;
using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Infrastructure.Contracts.Repository;

public interface ITransportTypesRepository
{
    Task<Result> Add(TransportTypesCollection collection, CancellationToken ct = default);
}
