using RemTechAvito.Core.FiltersManagement.TransportStates;
using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Infrastructure.Contracts.Parser;

public interface ITransportStatesParser
{
    Task<Result<TransportStatesCollection>> Parse(CancellationToken ct = default);
}
