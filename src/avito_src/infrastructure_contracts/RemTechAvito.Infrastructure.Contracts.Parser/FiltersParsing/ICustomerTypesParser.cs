using RemTechAvito.Core.FiltersManagement.CustomerTypes;
using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Infrastructure.Contracts.Parser;

public interface ICustomerTypesParser
{
    Task<Result<CustomerTypesCollection>> Parse(CancellationToken ct = default);
}
