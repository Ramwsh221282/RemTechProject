using RemTechAvito.Core.FiltersManagement.TransportTypes;
using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Infrastructure.Contracts.Parser;

public interface ITransportTypesParser
{
    Task<Result<TransportTypesCollection>> Parse(CancellationToken ct = default);
}
