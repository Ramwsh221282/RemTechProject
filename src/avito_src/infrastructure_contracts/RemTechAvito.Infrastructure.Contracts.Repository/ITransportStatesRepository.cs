using RemTechAvito.Core.FiltersManagement.TransportStates;
using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Infrastructure.Contracts.Repository;

public interface ITransportStatesRepository
{
    Task<Result> Add(TransportStatesCollection collection, CancellationToken ct = default);
}
