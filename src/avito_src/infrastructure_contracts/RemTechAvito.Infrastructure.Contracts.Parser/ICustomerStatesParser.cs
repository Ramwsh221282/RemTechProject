using RemTechAvito.Core.FiltersManagement.CustomerStates;
using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Infrastructure.Contracts.Parser;

public interface ICustomerStatesParser
{
    Task<Result<CustomerStatesCollection>> Parse(CancellationToken ct = default);
}
