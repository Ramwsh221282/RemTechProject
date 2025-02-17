using RemTechAvito.Core.FiltersManagement.TransportStates;
using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Infrastructure.Contracts.Parser.FiltersParsing;

public interface ITransportStatesParser
{
    IAsyncEnumerable<Result<TransportState>> Parse(CancellationToken ct = default);
}
