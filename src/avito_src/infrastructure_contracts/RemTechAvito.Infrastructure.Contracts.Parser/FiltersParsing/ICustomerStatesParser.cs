using RemTechAvito.Core.FiltersManagement.CustomerStates;
using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Infrastructure.Contracts.Parser.FiltersParsing;

public interface ICustomerStatesParser
{
    IAsyncEnumerable<Result<CustomerState>> Parse(CancellationToken ct = default);
}
